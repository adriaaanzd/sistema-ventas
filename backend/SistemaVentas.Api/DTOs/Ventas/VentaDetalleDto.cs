namespace SistemaVentas.Api.DTOs.Ventas;

public record VentaDetalleDto(
    int Id,
    DateTime Fecha,
    int ClienteId,
    string ClienteNombre,
    string ClienteDocumento,
    decimal Total,
    List<VentaLineaDto> Lineas);

public record VentaLineaDto(
    int ProductoId,
    string ProductoCodigo,
    string ProductoNombre,
    int Cantidad,
    decimal PrecioUnitario,
    decimal Subtotal);
