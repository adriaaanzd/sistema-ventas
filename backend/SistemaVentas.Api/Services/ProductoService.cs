using Microsoft.EntityFrameworkCore;
using SistemaVentas.Api.Data;
using SistemaVentas.Api.DTOs.Productos;
using SistemaVentas.Api.Errors;
using SistemaVentas.Api.Models;

namespace SistemaVentas.Api.Services;

public class ProductoService(VentasDbContext db)
{
    public async Task<List<ProductoDto>> ListarAsync(string? buscar)
    {
        var query = db.Productos.AsQueryable();

        if (!string.IsNullOrWhiteSpace(buscar))
        {
            var texto = buscar.Trim();
            query = query.Where(p => p.Nombre.Contains(texto) || p.Codigo.Contains(texto));
        }

        return await query.OrderBy(p => p.Nombre).Select(p => ADto(p)).ToListAsync();
    }

    public async Task<ProductoDto> ObtenerAsync(int id)
    {
        var producto = await db.Productos.FindAsync(id) ?? throw NoEncontrado(id);
        return ADto(producto);
    }

    public async Task<ProductoDto> CrearAsync(ProductoCrearDto dto)
    {
        var producto = new Producto();
        Copiar(dto, producto);
        await ValidarCodigoUnicoAsync(producto);

        db.Productos.Add(producto);
        await db.SaveChangesAsync();
        return ADto(producto);
    }

    public async Task ActualizarAsync(int id, ProductoCrearDto dto)
    {
        var producto = await db.Productos.FindAsync(id) ?? throw NoEncontrado(id);
        Copiar(dto, producto);
        await ValidarCodigoUnicoAsync(producto);

        await db.SaveChangesAsync();
    }

    public async Task EliminarAsync(int id)
    {
        var producto = await db.Productos.FindAsync(id) ?? throw NoEncontrado(id);

        if (await db.DetalleVentas.AnyAsync(d => d.ProductoId == id))
            throw new ConflictException($"No se puede eliminar {producto.Nombre} porque aparece en ventas registradas.");

        db.Productos.Remove(producto);
        await db.SaveChangesAsync();
    }

    // Id = 0 en un producto nuevo, así que la exclusión no descarta a nadie.
    private async Task ValidarCodigoUnicoAsync(Producto producto)
    {
        if (await db.Productos.AnyAsync(p => p.Codigo == producto.Codigo && p.Id != producto.Id))
            throw new ConflictException($"Ya existe un producto con el código {producto.Codigo}.");
    }

    private static void Copiar(ProductoCrearDto dto, Producto producto)
    {
        // decimal(18,2) redondearía en silencio 10.999 a 11.00; mejor rechazarlo.
        if (decimal.Round(dto.Precio, 2) != dto.Precio)
            throw new BusinessRuleException("El precio admite como máximo 2 decimales.");

        producto.Codigo = dto.Codigo.Trim().ToUpperInvariant();
        producto.Nombre = dto.Nombre.Trim();
        producto.Descripcion = string.IsNullOrWhiteSpace(dto.Descripcion) ? null : dto.Descripcion.Trim();
        producto.Precio = dto.Precio;
        producto.Stock = dto.Stock;
        producto.StockMinimo = dto.StockMinimo;
    }

    // Pública porque DashboardService la reutiliza para la lista de stock bajo.
    public static ProductoDto ADto(Producto p) =>
        new(p.Id, p.Codigo, p.Nombre, p.Descripcion, p.Precio, p.Stock, p.StockMinimo, p.Stock <= p.StockMinimo, p.FechaCreacion);

    private static NotFoundException NoEncontrado(int id) => new($"No existe un producto con Id {id}.");
}
