namespace SistemaVentas.Api.DTOs.Productos;

public record ProductoDto(
    int Id,
    string Codigo,
    string Nombre,
    string? Descripcion,
    decimal Precio,
    int Stock,
    int StockMinimo,
    bool StockBajo,
    DateTime FechaCreacion);
