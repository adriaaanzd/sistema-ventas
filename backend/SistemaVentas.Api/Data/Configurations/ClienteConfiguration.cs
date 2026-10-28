using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaVentas.Api.Models;

namespace SistemaVentas.Api.Data.Configurations;

public class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
{
    public void Configure(EntityTypeBuilder<Cliente> builder)
    {
        builder.ToTable("Clientes");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Nombre).IsRequired().HasMaxLength(100);
        builder.Property(c => c.Documento).IsRequired().HasMaxLength(20);
        builder.Property(c => c.Correo).IsRequired().HasMaxLength(150);
        builder.Property(c => c.Telefono).HasMaxLength(20);
        builder.Property(c => c.Direccion).HasMaxLength(200);
        builder.Property(c => c.FechaRegistro).HasDefaultValueSql("GETDATE()");

        builder.HasIndex(c => c.Correo).IsUnique();
        builder.HasIndex(c => c.Documento).IsUnique();
    }
}
