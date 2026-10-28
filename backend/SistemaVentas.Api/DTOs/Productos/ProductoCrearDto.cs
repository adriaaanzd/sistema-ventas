using System.ComponentModel.DataAnnotations;

namespace SistemaVentas.Api.DTOs.Productos;

// Se usa tanto en POST como en PUT: los campos editables son los mismos.
public class ProductoCrearDto
{
    [Required(ErrorMessage = "El código es obligatorio.")]
    // Acepta espacios alrededor porque el servicio los recorta antes de guardar.
    [RegularExpression(@"^\s*[A-Za-z0-9-]{2,20}\s*$",ErrorMessage = "El código debe tener entre 2 y 20 caracteres (letras, números o guion).")]
    public string Codigo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 100 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(300, ErrorMessage = "La descripción no puede superar 300 caracteres.")]
    public string? Descripcion { get; set; }

    // Límites en cultura invariante: en un servidor con cultura es-ES "0.01" se leería mal.
    [Range(typeof(decimal), "0.01", "9999999999999999.99", ParseLimitsInInvariantCulture = true, ConvertValueInInvariantCulture = true,
        ErrorMessage = "El precio debe ser mayor que 0.")]
    public decimal Precio { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo.")]
    public int Stock { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "El stock mínimo no puede ser negativo.")]
    public int StockMinimo { get; set; }
}
