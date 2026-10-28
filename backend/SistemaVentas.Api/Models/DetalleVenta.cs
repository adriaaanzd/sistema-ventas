namespace SistemaVentas.Api.Models;

public class DetalleVenta
{
    public int Id { get; set; }
    public int VentaId { get; set; }
    public int ProductoId { get; set; }
    public int Cantidad { get; set; }
    // Precio del producto en el momento de la venta; no cambia si luego se edita el producto.
    public decimal PrecioUnitario { get; set; }
    public decimal Subtotal { get; set; }

    public Venta Venta { get; set; } = null!;
    public Producto Producto { get; set; } = null!;
}
