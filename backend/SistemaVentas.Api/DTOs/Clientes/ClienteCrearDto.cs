using System.ComponentModel.DataAnnotations;

namespace SistemaVentas.Api.DTOs.Clientes;

// Se usa tanto en POST como en PUT: los campos editables son los mismos.
public class ClienteCrearDto
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 100 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El documento es obligatorio.")]
    [RegularExpression(@"^[A-Za-z0-9-]{6,20}$", ErrorMessage = "El documento debe tener entre 6 y 20 caracteres (letras, números o guion).")]
    public string Documento { get; set; } = string.Empty;

    [Required(ErrorMessage = "El correo es obligatorio.")]
    [StringLength(150, ErrorMessage = "El correo no puede superar 150 caracteres.")]
    [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "El correo no tiene un formato válido.")]
    public string Correo { get; set; } = string.Empty;

    [RegularExpression(@"^\+?[0-9\s-]{6,20}$", ErrorMessage = "El teléfono solo admite números, espacios, guiones y un + inicial (6 a 20 caracteres).")]
    public string? Telefono { get; set; }

    [StringLength(200, ErrorMessage = "La dirección no puede superar 200 caracteres.")]
    public string? Direccion { get; set; }
}
