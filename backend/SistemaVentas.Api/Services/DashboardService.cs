using Microsoft.EntityFrameworkCore;
using SistemaVentas.Api.Data;
using SistemaVentas.Api.DTOs.Dashboard;

namespace SistemaVentas.Api.Services;

public class DashboardService(VentasDbContext db)
{
    private const int MesesDelGrafico = 12;

    // DbContext no admite consultas en paralelo, por eso cada await va en secuencia.
    public async Task<DashboardDto> ObtenerAsync()
    {
        var totalClientes = await db.Clientes.CountAsync();
        var totalProductos = await db.Productos.CountAsync();
        var totalVentas = await db.Ventas.CountAsync();
        var ingresos = await db.Ventas.SumAsync(v => (decimal?)v.Total) ?? 0m;

        var hoy = DateTime.Today;
        var desde = new DateTime(hoy.Year, hoy.Month, 1).AddMonths(-(MesesDelGrafico - 1));
        var agrupado = await db.Ventas
            .Where(v => v.Fecha >= desde)
            .GroupBy(v => new { v.Fecha.Year, v.Fecha.Month })
            .Select(g => new { g.Key.Year, g.Key.Month, Cantidad = g.Count(), Total = g.Sum(v => v.Total) })
            .ToListAsync();

        // Los meses sin ventas también aparecen, con 0, para que el gráfico no salte meses.
        var ventasPorMes = Enumerable.Range(0, MesesDelGrafico)
            .Select(i => desde.AddMonths(i))
            .Select(mes =>
            {
                var datos = agrupado.FirstOrDefault(a => a.Year == mes.Year && a.Month == mes.Month);
                return new VentasPorMesDto(mes.ToString("yyyy-MM"), datos?.Cantidad ?? 0, datos?.Total ?? 0m);
            })
            .ToList();

        var ultimasVentas = await db.Ventas
            .OrderByDescending(v => v.Fecha).ThenByDescending(v => v.Id)
            .Take(5)
            .Select(VentaService.AResumen)
            .ToListAsync();

        var stockBajo = await db.Productos
            .Where(p => p.Stock <= p.StockMinimo)
            .OrderBy(p => p.Stock).ThenBy(p => p.Nombre)
            .Select(p => ProductoService.ADto(p))
            .ToListAsync();

        return new DashboardDto(totalClientes, totalProductos, totalVentas, ingresos, ventasPorMes, ultimasVentas, stockBajo);
    }
}
