using Microsoft.EntityFrameworkCore;
using SistemaVentas.Api.Models;

namespace SistemaVentas.Api.Data;

// Fechas fijas a propósito: con DateTime.Now cada migración generaría cambios nuevos.
public static class SeedData
{
    public static void Aplicar(ModelBuilder modelBuilder)
    {
        var clientes = new[]
        {
            new Cliente { Id = 1, Nombre = "María Fernanda López Quispe", Documento = "45781236", Correo = "maria.lopez@example.com", Telefono = "987654321", Direccion = "Av. Los Olivos 245", FechaRegistro = new DateTime(2026, 1, 8, 9, 30, 0) },
            new Cliente { Id = 2, Nombre = "Carlos Alberto Ramírez Torres", Documento = "41236589", Correo = "carlos.ramirez@example.com", Telefono = "912345678", Direccion = "Jr. Las Begonias 1180", FechaRegistro = new DateTime(2026, 1, 15, 11, 0, 0) },
            new Cliente { Id = 3, Nombre = "Lucía Gabriela Mendoza Vargas", Documento = "70125463", Correo = "lucia.mendoza@example.com", Telefono = "998877665", Direccion = "Calle Los Pinos 332", FechaRegistro = new DateTime(2026, 1, 27, 16, 45, 0) },
            new Cliente { Id = 4, Nombre = "José Miguel Castillo Rojas", Documento = "46987512", Correo = "jose.castillo@example.com", Telefono = "923456781", Direccion = "Av. Primavera 870", FechaRegistro = new DateTime(2026, 2, 3, 10, 15, 0) },
            new Cliente { Id = 5, Nombre = "Ana Sofía Gutiérrez Paredes", Documento = "72451368", Correo = "ana.gutierrez@example.com", Telefono = "956781234", Direccion = "Jr. Huáscar 455", FechaRegistro = new DateTime(2026, 2, 19, 15, 20, 0) },
            new Cliente { Id = 6, Nombre = "Diego Armando Flores Huamán", Documento = "43659871", Correo = "diego.flores@example.com", Telefono = "934567812", Direccion = "Av. Universitaria 2010", FechaRegistro = new DateTime(2026, 3, 2, 12, 10, 0) },
            new Cliente { Id = 7, Nombre = "Valeria Alejandra Chávez Ríos", Documento = "75896412", Correo = "valeria.chavez@example.com", Telefono = "945678123", Direccion = "Calle Las Magnolias 118", FechaRegistro = new DateTime(2026, 3, 18, 17, 35, 0) },
            new Cliente { Id = 8, Nombre = "Jorge Luis Sánchez Medina", Documento = "40789563", Correo = "jorge.sanchez@example.com", Telefono = "967812345", Direccion = "Av. Colonial 1530", FechaRegistro = new DateTime(2026, 4, 6, 9, 50, 0) },
            new Cliente { Id = 9, Nombre = "Camila Andrea Torres Salazar", Documento = "71236984", Correo = "camila.torres@example.com", Telefono = "978123456", Direccion = "Jr. Junín 640", FechaRegistro = new DateTime(2026, 4, 22, 14, 5, 0) },
            new Cliente { Id = 10, Nombre = "Renzo Martín Díaz Cárdenas", Documento = "44521789", Correo = "renzo.diaz@example.com", Telefono = "989123456", Direccion = "Av. La Marina 2875", FechaRegistro = new DateTime(2026, 5, 2, 18, 25, 0) }
        };

        // Stock actual (ya descontadas las ventas de ejemplo). Productos 2, 4, 9, 13 y 15 quedan con stock bajo.
        var productos = new[]
        {
            new Producto { Id = 1, Codigo = "LAP-001", Nombre = "Laptop Lenovo IdeaPad 3", Descripcion = "15.6\", Ryzen 5, 8 GB RAM, 512 GB SSD", Precio = 2499.00m, Stock = 8, StockMinimo = 3, FechaCreacion = new DateTime(2026, 1, 5, 8, 0, 0) },
            new Producto { Id = 2, Codigo = "LAP-002", Nombre = "Laptop HP Pavilion 15", Descripcion = "15.6\", Core i7, 16 GB RAM, 1 TB SSD", Precio = 3199.00m, Stock = 2, StockMinimo = 3, FechaCreacion = new DateTime(2026, 1, 5, 8, 0, 0) },
            new Producto { Id = 3, Codigo = "MON-001", Nombre = "Monitor LG 24\" Full HD", Descripcion = "Panel IPS, 75 Hz, HDMI", Precio = 549.90m, Stock = 15, StockMinimo = 5, FechaCreacion = new DateTime(2026, 1, 5, 8, 0, 0) },
            new Producto { Id = 4, Codigo = "MON-002", Nombre = "Monitor Samsung 27\" Curvo", Descripcion = "Panel VA, 75 Hz, FreeSync", Precio = 899.00m, Stock = 4, StockMinimo = 4, FechaCreacion = new DateTime(2026, 1, 5, 8, 0, 0) },
            new Producto { Id = 5, Codigo = "TEC-001", Nombre = "Teclado mecánico Redragon Kumara", Descripcion = "Switches Outemu Blue, retroiluminado", Precio = 189.90m, Stock = 25, StockMinimo = 10, FechaCreacion = new DateTime(2026, 1, 5, 8, 0, 0) },
            new Producto { Id = 6, Codigo = "MOU-001", Nombre = "Mouse inalámbrico Logitech M185", Descripcion = "Receptor USB, 1000 DPI", Precio = 59.90m, Stock = 40, StockMinimo = 10, FechaCreacion = new DateTime(2026, 1, 5, 8, 0, 0) },
            new Producto { Id = 7, Codigo = "MOU-002", Nombre = "Mouse gamer Razer DeathAdder", Descripcion = "Sensor óptico 6400 DPI", Precio = 219.00m, Stock = 6, StockMinimo = 5, FechaCreacion = new DateTime(2026, 1, 12, 8, 0, 0) },
            new Producto { Id = 8, Codigo = "AUD-001", Nombre = "Audífonos Sony WH-CH520", Descripcion = "Bluetooth, 50 h de batería", Precio = 199.00m, Stock = 12, StockMinimo = 5, FechaCreacion = new DateTime(2026, 1, 12, 8, 0, 0) },
            new Producto { Id = 9, Codigo = "IMP-001", Nombre = "Impresora Epson EcoTank L3250", Descripcion = "Multifuncional, Wi-Fi, tinta continua", Precio = 899.00m, Stock = 1, StockMinimo = 2, FechaCreacion = new DateTime(2026, 1, 12, 8, 0, 0) },
            new Producto { Id = 10, Codigo = "ALM-001", Nombre = "Disco SSD Kingston 1 TB", Descripcion = "NVMe PCIe 4.0", Precio = 279.90m, Stock = 20, StockMinimo = 8, FechaCreacion = new DateTime(2026, 2, 2, 8, 0, 0) },
            new Producto { Id = 11, Codigo = "ALM-002", Nombre = "Memoria USB SanDisk 64 GB", Descripcion = "USB 3.2", Precio = 35.90m, Stock = 60, StockMinimo = 20, FechaCreacion = new DateTime(2026, 2, 2, 8, 0, 0) },
            new Producto { Id = 12, Codigo = "RED-001", Nombre = "Router TP-Link Archer C6", Descripcion = "Doble banda AC1200", Precio = 169.00m, Stock = 9, StockMinimo = 4, FechaCreacion = new DateTime(2026, 2, 2, 8, 0, 0) },
            new Producto { Id = 13, Codigo = "CAM-001", Nombre = "Webcam Logitech C920", Descripcion = "Full HD 1080p con micrófono", Precio = 329.00m, Stock = 3, StockMinimo = 5, FechaCreacion = new DateTime(2026, 2, 16, 8, 0, 0) },
            new Producto { Id = 14, Codigo = "ACC-001", Nombre = "Base refrigerante para laptop", Descripcion = "5 ventiladores, hasta 17\"", Precio = 79.90m, Stock = 18, StockMinimo = 6, FechaCreacion = new DateTime(2026, 2, 16, 8, 0, 0) },
            new Producto { Id = 15, Codigo = "ACC-002", Nombre = "Hub USB-C 7 en 1", Descripcion = "HDMI 4K, 3 USB, lector SD", Precio = 129.00m, Stock = 0, StockMinimo = 3, FechaCreacion = new DateTime(2026, 2, 16, 8, 0, 0) }
        };

        var cabeceras = new (int Id, int ClienteId, DateTime Fecha)[]
        {
            (1, 1, new DateTime(2026, 5, 6, 10, 15, 0)),
            (2, 2, new DateTime(2026, 5, 21, 16, 40, 0)),
            (3, 3, new DateTime(2026, 6, 10, 11, 5, 0)),
            (4, 4, new DateTime(2026, 6, 27, 18, 20, 0)),
            (5, 5, new DateTime(2026, 7, 8, 9, 45, 0)),
            (6, 1, new DateTime(2026, 7, 30, 15, 30, 0)),
            (7, 6, new DateTime(2026, 8, 14, 12, 10, 0)),
            (8, 7, new DateTime(2026, 9, 3, 17, 0, 0)),
            (9, 8, new DateTime(2026, 9, 25, 10, 30, 0)),
            (10, 9, new DateTime(2026, 10, 15, 14, 50, 0))
        };

        var lineas = new (int VentaId, int ProductoId, int Cantidad)[]
        {
            (1, 1, 1), (1, 6, 1),
            (2, 3, 2), (2, 5, 1),
            (3, 8, 1), (3, 11, 3),
            (4, 2, 1), (4, 14, 1),
            (5, 9, 1), (5, 11, 2),
            (6, 10, 2), (6, 14, 1),
            (7, 4, 1), (7, 7, 1),
            (8, 13, 2), (8, 15, 1),
            (9, 1, 1), (9, 5, 1), (9, 6, 2),
            (10, 3, 1), (10, 8, 1)
        };

        // Subtotales y totales se calculan igual que lo hará el backend, en lugar de escribirlos a mano.
        var precios = productos.ToDictionary(p => p.Id, p => p.Precio);
        var detalles = lineas.Select((l, i) => new DetalleVenta
        {
            Id = i + 1,
            VentaId = l.VentaId,
            ProductoId = l.ProductoId,
            Cantidad = l.Cantidad,
            PrecioUnitario = precios[l.ProductoId],
            Subtotal = l.Cantidad * precios[l.ProductoId]
        }).ToArray();

        var ventas = cabeceras.Select(c => new Venta
        {
            Id = c.Id,
            ClienteId = c.ClienteId,
            Fecha = c.Fecha,
            Total = detalles.Where(d => d.VentaId == c.Id).Sum(d => d.Subtotal)
        }).ToArray();

        modelBuilder.Entity<Cliente>().HasData(clientes);
        modelBuilder.Entity<Producto>().HasData(productos);
        modelBuilder.Entity<Venta>().HasData(ventas);
        modelBuilder.Entity<DetalleVenta>().HasData(detalles);
    }
}
