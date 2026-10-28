using Microsoft.AspNetCore.Mvc;
using SistemaVentas.Api.DTOs.Ventas;
using SistemaVentas.Api.Services;

namespace SistemaVentas.Api.Controllers;

// Sin PUT ni DELETE: una venta registrada es un documento contable que no se modifica.
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class VentasController(VentaService service) : ControllerBase
{
    /// <summary>Historial de ventas, de la más reciente a la más antigua.</summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<VentaResumenDto>>> Listar() =>
        Ok(await service.ListarAsync());

    /// <summary>Cabecera y líneas de una venta.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VentaDetalleDto>> Obtener(int id) =>
        Ok(await service.ObtenerAsync(id));

    /// <summary>Registra una venta: calcula precios y total, valida y descuenta stock en una sola transacción.</summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<VentaDetalleDto>> Crear(VentaCrearDto dto)
    {
        var creada = await service.CrearAsync(dto);
        return CreatedAtAction(nameof(Obtener), new { id = creada.Id }, creada);
    }
}
