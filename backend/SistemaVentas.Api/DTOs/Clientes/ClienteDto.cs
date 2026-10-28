namespace SistemaVentas.Api.DTOs.Clientes;

public record ClienteDto(
    int Id,
    string Nombre,
    string Documento,
    string Correo,
    string? Telefono,
    string? Direccion,
    DateTime FechaRegistro);
