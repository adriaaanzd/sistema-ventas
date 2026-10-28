using Microsoft.EntityFrameworkCore;
using SistemaVentas.Api.Models;

namespace SistemaVentas.Api.Data;

public class VentasDbContext(DbContextOptions<VentasDbContext> options) : DbContext(options)
{
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<Venta> Ventas => Set<Venta>();
    public DbSet<DetalleVenta> DetalleVentas => Set<DetalleVenta>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(VentasDbContext).Assembly);
        SeedData.Aplicar(modelBuilder);
    }
}
