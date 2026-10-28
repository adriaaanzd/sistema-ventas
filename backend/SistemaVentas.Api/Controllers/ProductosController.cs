using Microsoft.AspNetCore.Mvc;
using SistemaVentas.Api.DTOs.Productos;
using SistemaVentas.Api.Services;

namespace SistemaVentas.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ProductosController(ProductoService service) : ControllerBase
{
    /// <summary>Lista los productos; filtra por nombre o código si se envía <paramref name="buscar"/>.</summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ProductoDto>>> Listar([FromQuery] string? buscar) =>
        Ok(await service.ListarAsync(buscar));

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductoDto>> Obtener(int id) =>
        Ok(await service.ObtenerAsync(id));

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ProductoDto>> Crear(ProductoCrearDto dto)
    {
        var creado = await service.CrearAsync(dto);
        return CreatedAtAction(nameof(Obtener), new { id = creado.Id }, creado);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Actualizar(int id, ProductoCrearDto dto)
    {
        await service.ActualizarAsync(id, dto);
        return NoContent();
    }

    /// <summary>Elimina un producto; responde 409 si aparece en alguna venta.</summary>
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
