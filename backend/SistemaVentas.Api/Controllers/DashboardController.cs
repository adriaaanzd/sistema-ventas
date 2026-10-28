using Microsoft.AspNetCore.Mvc;
using SistemaVentas.Api.DTOs.Dashboard;
using SistemaVentas.Api.Services;

namespace SistemaVentas.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class DashboardController(DashboardService service) : ControllerBase
{
    /// <summary>Indicadores, ventas de los últimos 12 meses, últimas 5 ventas y productos con stock bajo.</summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<DashboardDto>> Obtener() =>
        Ok(await service.ObtenerAsync());
}
