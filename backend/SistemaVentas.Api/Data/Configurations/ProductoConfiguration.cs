using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaVentas.Api.Models;

namespace SistemaVentas.Api.Data.Configurations;

public class ProductoConfiguration : IEntityTypeConfiguration<Producto>
{
    public void Configure(EntityTypeBuilder<Producto> builder)
    {
        builder.ToTable("Productos", t =>
        {
            t.HasCheckConstraint("CK_Productos_Precio", "[Precio] > 0");
            t.HasCheckConstraint("CK_Productos_Stock", "[Stock] >= 0");
            t.HasCheckConstraint("CK_Productos_StockMinimo", "[StockMinimo] >= 0");
        });
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Codigo).IsRequired().HasMaxLength(20);
        builder.Property(p => p.Nombre).IsRequired().HasMaxLength(100);
        builder.Property(p => p.Descripcion).HasMaxLength(300);
        builder.Property(p => p.Precio).HasPrecision(18, 2);
        builder.Property(p => p.FechaCreacion).HasDefaultValueSql("GETDATE()");

        builder.HasIndex(p => p.Codigo).IsUnique();
    }
}
