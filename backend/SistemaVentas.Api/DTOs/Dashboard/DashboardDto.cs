using SistemaVentas.Api.DTOs.Productos;
using SistemaVentas.Api.DTOs.Ventas;

namespace SistemaVentas.Api.DTOs.Dashboard;

public record DashboardDto(
    int TotalClientes,
    int TotalProductos,
    int TotalVentas,
    decimal Ingresos,
    List<VentasPorMesDto> VentasPorMes,
    List<VentaResumenDto> UltimasVentas,
    List<ProductoDto> ProductosStockBajo);

/// <param name="Mes">Formato yyyy-MM.</param>
public record VentasPorMesDto(string Mes, int Cantidad, decimal Total);
