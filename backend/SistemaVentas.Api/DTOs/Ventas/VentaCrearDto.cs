using System.ComponentModel.DataAnnotations;

namespace SistemaVentas.Api.DTOs.Ventas;

// No lleva precios ni total: el backend los calcula con los precios vigentes.
public class VentaCrearDto
{
    [Range(1, int.MaxValue, ErrorMessage = "Seleccione un cliente.")]
    public int ClienteId { get; set; }

    [Required(ErrorMessage = "La venta debe tener al menos un producto.")]
    [MinLength(1, ErrorMessage = "La venta debe tener al menos un producto.")]
    public List<VentaLineaCrearDto> Lineas { get; set; } = [];
}

public class VentaLineaCrearDto
{
    [Range(1, int.MaxValue, ErrorMessage = "Seleccione un producto.")]
    public int ProductoId { get; set; }

    [Range(1, 10000, ErrorMessage = "La cantidad debe estar entre 1 y 10000.")]
    public int Cantidad { get; set; }
}
