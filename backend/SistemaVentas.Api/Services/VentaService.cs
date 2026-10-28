using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SistemaVentas.Api.Data;
using SistemaVentas.Api.DTOs.Ventas;
using SistemaVentas.Api.Errors;
using SistemaVentas.Api.Models;

namespace SistemaVentas.Api.Services;

public class VentaService(VentasDbContext db)
{
    // Proyección compartida con DashboardService; EF la traduce a un solo SELECT.
    public static readonly Expression<Func<Venta, VentaResumenDto>> AResumen = v =>
        new VentaResumenDto(v.Id, v.Fecha, v.ClienteId, v.Cliente.Nombre, v.Detalles.Sum(d => d.Cantidad), v.Total);

    public async Task<List<VentaResumenDto>> ListarAsync() =>
        await db.Ventas
            .OrderByDescending(v => v.Fecha).ThenByDescending(v => v.Id)
            .Select(AResumen)
            .ToListAsync();

    public async Task<VentaDetalleDto> ObtenerAsync(int id) =>
        await db.Ventas
            .Where(v => v.Id == id)
            .Select(v => new VentaDetalleDto(
                v.Id, v.Fecha, v.ClienteId, v.Cliente.Nombre, v.Cliente.Documento, v.Total,
                v.Detalles.OrderBy(d => d.Id)
                    .Select(d => new VentaLineaDto(d.ProductoId, d.Producto.Codigo, d.Producto.Nombre, d.Cantidad, d.PrecioUnitario, d.Subtotal))
                    .ToList()))
            .FirstOrDefaultAsync()
        ?? throw new NotFoundException($"No existe una venta con Id {id}.");

    public async Task<VentaDetalleDto> CrearAsync(VentaCrearDto dto)
    {
        // Un producto repetido en el carrito se suma en una sola línea. Ordenar por Id hace que
        // dos ventas simultáneas bloqueen las filas en el mismo orden y no se produzca un deadlock.
        var lineas = dto.Lineas
            .GroupBy(l => l.ProductoId)
            .Select(g => new { ProductoId = g.Key, Cantidad = g.Sum(l => l.Cantidad) })
            .OrderBy(l => l.ProductoId)
            .ToList();

        if (!await db.Clientes.AnyAsync(c => c.Id == dto.ClienteId))
            throw new BusinessRuleException($"No existe un cliente con Id {dto.ClienteId}.");

        var ids = lineas.Select(l => l.ProductoId).ToList();
        var productos = await db.Productos.AsNoTracking()
            .Where(p => ids.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id);

        var inexistentes = ids.Where(id => !productos.ContainsKey(id)).ToList();
        if (inexistentes.Count > 0)
            throw new BusinessRuleException($"No existen productos con Id: {string.Join(", ", inexistentes)}.");

        // Primera comprobación, para devolver todos los faltantes en un solo mensaje.
        var sinStock = lineas
            .Where(l => productos[l.ProductoId].Stock < l.Cantidad)
            .Select(l => $"{productos[l.ProductoId].Nombre} (disponible: {productos[l.ProductoId].Stock}, solicitado: {l.Cantidad})")
            .ToList();
        if (sinStock.Count > 0)
            throw new BusinessRuleException($"Stock insuficiente: {string.Join("; ", sinStock)}.");

        // Si algo lanza una excepción antes de CommitAsync, al salir del using se hace rollback de todo.
        await using var transaccion = await db.Database.BeginTransactionAsync();

        foreach (var linea in lineas)
        {
            // UPDATE atómico con condición: si otra venta se llevó el stock entre la lectura y aquí,
            // no se actualiza ninguna fila y la venta se cancela en lugar de dejar stock negativo.
            var filas = await db.Productos
                .Where(p => p.Id == linea.ProductoId && p.Stock >= linea.Cantidad)
                .ExecuteUpdateAsync(s => s.SetProperty(p => p.Stock, p => p.Stock - linea.Cantidad));

            if (filas == 0)
                throw new BusinessRuleException($"Stock insuficiente para {productos[linea.ProductoId].Nombre}: otra venta acaba de consumirlo.");
        }

        var venta = new Venta
        {
            ClienteId = dto.ClienteId,
            Detalles = lineas.Select(l => new DetalleVenta
            {
                ProductoId = l.ProductoId,
                Cantidad = l.Cantidad,
                PrecioUnitario = productos[l.ProductoId].Precio,
                Subtotal = l.Cantidad * productos[l.ProductoId].Precio
            }).ToList()
        };
        venta.Total = venta.Detalles.Sum(d => d.Subtotal);

        db.Ventas.Add(venta);
        await db.SaveChangesAsync();
        await transaccion.CommitAsync();

        return await ObtenerAsync(venta.Id);
    }
}
