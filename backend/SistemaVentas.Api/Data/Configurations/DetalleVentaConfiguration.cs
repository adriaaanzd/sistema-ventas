using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaVentas.Api.Models;

namespace SistemaVentas.Api.Data.Configurations;

public class DetalleVentaConfiguration : IEntityTypeConfiguration<DetalleVenta>
{
    public void Configure(EntityTypeBuilder<DetalleVenta> builder)
    {
        builder.ToTable("DetalleVentas", t =>
        {
            t.HasCheckConstraint("CK_DetalleVentas_Cantidad", "[Cantidad] > 0");
            t.HasCheckConstraint("CK_DetalleVentas_PrecioUnitario", "[PrecioUnitario] > 0");
            t.HasCheckConstraint("CK_DetalleVentas_Subtotal", "[Subtotal] = [Cantidad] * [PrecioUnitario]");
        });
        builder.HasKey(d => d.Id);

        builder.Property(d => d.PrecioUnitario).HasPrecision(18, 2);
        builder.Property(d => d.Subtotal).HasPrecision(18, 2);

        // Las líneas pertenecen a la venta: no existen sin ella.
        builder.HasOne(d => d.Venta)
            .WithMany(v => v.Detalles)
            .HasForeignKey(d => d.VentaId)
            .OnDelete(DeleteBehavior.Cascade);

        // Un producto vendido no se puede borrar: rompería el historial.
        builder.HasOne(d => d.Producto)
            .WithMany(p => p.Detalles)
            .HasForeignKey(d => d.ProductoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
