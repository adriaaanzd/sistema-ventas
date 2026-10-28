using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaVentas.Api.Models;

namespace SistemaVentas.Api.Data.Configurations;

public class VentaConfiguration : IEntityTypeConfiguration<Venta>
{
    public void Configure(EntityTypeBuilder<Venta> builder)
    {
        builder.ToTable("Ventas", t => t.HasCheckConstraint("CK_Ventas_Total", "[Total] > 0"));
        builder.HasKey(v => v.Id);

        builder.Property(v => v.Total).HasPrecision(18, 2);
        builder.Property(v => v.Fecha).HasDefaultValueSql("GETDATE()");

        // Un cliente con ventas no se puede borrar: la venta es un registro histórico.
        builder.HasOne(v => v.Cliente)
            .WithMany(c => c.Ventas)
            .HasForeignKey(v => v.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(v => v.Fecha);
    }
}
