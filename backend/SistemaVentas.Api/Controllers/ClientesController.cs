using Microsoft.AspNetCore.Mvc;
using SistemaVentas.Api.DTOs.Clientes;
using SistemaVentas.Api.Services;

namespace SistemaVentas.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ClientesController(ClienteService service) : ControllerBase
{
    /// <summary>Lista los clientes; filtra por nombre, documento o correo si se envía <paramref name="buscar"/>.</summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ClienteDto>>> Listar([FromQuery] string? buscar) =>
        Ok(await service.ListarAsync(buscar));

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ClienteDto>> Obtener(int id) =>
        Ok(await service.ObtenerAsync(id));

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ClienteDto>> Crear(ClienteCrearDto dto)
    {
        var creado = await service.CrearAsync(dto);
        return CreatedAtAction(nameof(Obtener), new { id = creado.Id }, creado);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Actualizar(int id, ClienteCrearDto dto)
    {
        await service.ActualizarAsync(id, dto);
        return NoContent();
    }

    /// <summary>Elimina un cliente; responde 409 si tiene ventas asociadas.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Eliminar(int id)
    {
        await service.EliminarAsync(id);
        return NoContent();
    }
}
