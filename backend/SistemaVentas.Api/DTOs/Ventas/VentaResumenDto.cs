namespace SistemaVentas.Api.DTOs.Ventas;

public record VentaResumenDto(
    int Id,
    DateTime Fecha,
    int ClienteId,
    string ClienteNombre,
    int CantidadProductos,
    decimal Total);
