using Microsoft.EntityFrameworkCore;
using SistemaVentas.Api.Data;
using SistemaVentas.Api.DTOs.Clientes;
using SistemaVentas.Api.Errors;
using SistemaVentas.Api.Models;

namespace SistemaVentas.Api.Services;

public class ClienteService(VentasDbContext db)
{
    public async Task<List<ClienteDto>> ListarAsync(string? buscar)
    {
        var query = db.Clientes.AsQueryable();

        if (!string.IsNullOrWhiteSpace(buscar))
        {
            var texto = buscar.Trim();
            query = query.Where(c => c.Nombre.Contains(texto) || c.Documento.Contains(texto) || c.Correo.Contains(texto));
        }

        return await query.OrderBy(c => c.Nombre).Select(c => ADto(c)).ToListAsync();
    }

    public async Task<ClienteDto> ObtenerAsync(int id)
    {
        var cliente = await db.Clientes.FindAsync(id) ?? throw NoEncontrado(id);
        return ADto(cliente);
    }

    public async Task<ClienteDto> CrearAsync(ClienteCrearDto dto)
    {
        var cliente = new Cliente();
        Copiar(dto, cliente);
        await ValidarUnicosAsync(cliente);

        db.Clientes.Add(cliente);
        await db.SaveChangesAsync();
        return ADto(cliente);
    }

    public async Task ActualizarAsync(int id, ClienteCrearDto dto)
    {
        var cliente = await db.Clientes.FindAsync(id) ?? throw NoEncontrado(id);
        Copiar(dto, cliente);
        await ValidarUnicosAsync(cliente);

        await db.SaveChangesAsync();
    }

    public async Task EliminarAsync(int id)
    {
        var cliente = await db.Clientes.FindAsync(id) ?? throw NoEncontrado(id);

        if (await db.Ventas.AnyAsync(v => v.ClienteId == id))
            throw new ConflictException($"No se puede eliminar a {cliente.Nombre} porque tiene ventas registradas.");

        db.Clientes.Remove(cliente);
        await db.SaveChangesAsync();
    }

    // Id = 0 en un cliente nuevo, así que la exclusión no descarta a nadie.
    private async Task ValidarUnicosAsync(Cliente cliente)
    {
        if (await db.Clientes.AnyAsync(c => c.Correo == cliente.Correo && c.Id != cliente.Id))
            throw new ConflictException($"Ya existe un cliente con el correo {cliente.Correo}.");

        if (await db.Clientes.AnyAsync(c => c.Documento == cliente.Documento && c.Id != cliente.Id))
            throw new ConflictException($"Ya existe un cliente con el documento {cliente.Documento}.");
    }

    private static void Copiar(ClienteCrearDto dto, Cliente cliente)
    {
        cliente.Nombre = dto.Nombre.Trim();
        cliente.Documento = dto.Documento.Trim().ToUpperInvariant();
        cliente.Correo = dto.Correo.Trim().ToLowerInvariant();
        cliente.Telefono = string.IsNullOrWhiteSpace(dto.Telefono) ? null : dto.Telefono.Trim();
        cliente.Direccion = string.IsNullOrWhiteSpace(dto.Direccion) ? null : dto.Direccion.Trim();
    }

    private static ClienteDto ADto(Cliente c) =>
        new(c.Id, c.Nombre, c.Documento, c.Correo, c.Telefono, c.Direccion, c.FechaRegistro);

    private static NotFoundException NoEncontrado(int id) => new($"No existe un cliente con Id {id}.");
}
