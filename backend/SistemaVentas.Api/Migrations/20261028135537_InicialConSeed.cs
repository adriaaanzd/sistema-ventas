using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SistemaVentas.Api.Migrations
{
    /// <inheritdoc />
    public partial class InicialConSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Documento = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Correo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Direccion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Productos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    Precio = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Stock = table.Column<int>(type: "int", nullable: false),
                    StockMinimo = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Productos", x => x.Id);
                    table.CheckConstraint("CK_Productos_Precio", "[Precio] > 0");
                    table.CheckConstraint("CK_Productos_Stock", "[Stock] >= 0");
                    table.CheckConstraint("CK_Productos_StockMinimo", "[StockMinimo] >= 0");
                });

            migrationBuilder.CreateTable(
                name: "Ventas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClienteId = table.Column<int>(type: "int", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    Total = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ventas", x => x.Id);
                    table.CheckConstraint("CK_Ventas_Total", "[Total] > 0");
                    table.ForeignKey(
                        name: "FK_Ventas_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DetalleVentas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VentaId = table.Column<int>(type: "int", nullable: false),
                    ProductoId = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    PrecioUnitario = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Subtotal = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetalleVentas", x => x.Id);
                    table.CheckConstraint("CK_DetalleVentas_Cantidad", "[Cantidad] > 0");
                    table.CheckConstraint("CK_DetalleVentas_PrecioUnitario", "[PrecioUnitario] > 0");
                    table.CheckConstraint("CK_DetalleVentas_Subtotal", "[Subtotal] = [Cantidad] * [PrecioUnitario]");
                    table.ForeignKey(
                        name: "FK_DetalleVentas_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DetalleVentas_Ventas_VentaId",
                        column: x => x.VentaId,
                        principalTable: "Ventas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Clientes",
                columns: new[] { "Id", "Correo", "Direccion", "Documento", "FechaRegistro", "Nombre", "Telefono" },
                values: new object[,]
                {
                    { 1, "maria.lopez@example.com", "Av. Los Olivos 245", "45781236", new DateTime(2026, 1, 8, 9, 30, 0, 0, DateTimeKind.Unspecified), "María Fernanda López Quispe", "987654321" },
                    { 2, "carlos.ramirez@example.com", "Jr. Las Begonias 1180", "41236589", new DateTime(2026, 1, 15, 11, 0, 0, 0, DateTimeKind.Unspecified), "Carlos Alberto Ramírez Torres", "912345678" },
                    { 3, "lucia.mendoza@example.com", "Calle Los Pinos 332", "70125463", new DateTime(2026, 1, 27, 16, 45, 0, 0, DateTimeKind.Unspecified), "Lucía Gabriela Mendoza Vargas", "998877665" },
                    { 4, "jose.castillo@example.com", "Av. Primavera 870", "46987512", new DateTime(2026, 2, 3, 10, 15, 0, 0, DateTimeKind.Unspecified), "José Miguel Castillo Rojas", "923456781" },
                    { 5, "ana.gutierrez@example.com", "Jr. Huáscar 455", "72451368", new DateTime(2026, 2, 19, 15, 20, 0, 0, DateTimeKind.Unspecified), "Ana Sofía Gutiérrez Paredes", "956781234" },
                    { 6, "diego.flores@example.com", "Av. Universitaria 2010", "43659871", new DateTime(2026, 3, 2, 12, 10, 0, 0, DateTimeKind.Unspecified), "Diego Armando Flores Huamán", "934567812" },
                    { 7, "valeria.chavez@example.com", "Calle Las Magnolias 118", "75896412", new DateTime(2026, 3, 18, 17, 35, 0, 0, DateTimeKind.Unspecified), "Valeria Alejandra Chávez Ríos", "945678123" },
                    { 8, "jorge.sanchez@example.com", "Av. Colonial 1530", "40789563", new DateTime(2026, 4, 6, 9, 50, 0, 0, DateTimeKind.Unspecified), "Jorge Luis Sánchez Medina", "967812345" },
                    { 9, "camila.torres@example.com", "Jr. Junín 640", "71236984", new DateTime(2026, 4, 22, 14, 5, 0, 0, DateTimeKind.Unspecified), "Camila Andrea Torres Salazar", "978123456" },
                    { 10, "renzo.diaz@example.com", "Av. La Marina 2875", "44521789", new DateTime(2026, 5, 2, 18, 25, 0, 0, DateTimeKind.Unspecified), "Renzo Martín Díaz Cárdenas", "989123456" }
                });

            migrationBuilder.InsertData(
                table: "Productos",
                columns: new[] { "Id", "Codigo", "Descripcion", "FechaCreacion", "Nombre", "Precio", "Stock", "StockMinimo" },
                values: new object[,]
                {
                    { 1, "LAP-001", "15.6\", Ryzen 5, 8 GB RAM, 512 GB SSD", new DateTime(2026, 1, 5, 8, 0, 0, 0, DateTimeKind.Unspecified), "Laptop Lenovo IdeaPad 3", 2499.00m, 8, 3 },
                    { 2, "LAP-002", "15.6\", Core i7, 16 GB RAM, 1 TB SSD", new DateTime(2026, 1, 5, 8, 0, 0, 0, DateTimeKind.Unspecified), "Laptop HP Pavilion 15", 3199.00m, 2, 3 },
                    { 3, "MON-001", "Panel IPS, 75 Hz, HDMI", new DateTime(2026, 1, 5, 8, 0, 0, 0, DateTimeKind.Unspecified), "Monitor LG 24\" Full HD", 549.90m, 15, 5 },
                    { 4, "MON-002", "Panel VA, 75 Hz, FreeSync", new DateTime(2026, 1, 5, 8, 0, 0, 0, DateTimeKind.Unspecified), "Monitor Samsung 27\" Curvo", 899.00m, 4, 4 },
                    { 5, "TEC-001", "Switches Outemu Blue, retroiluminado", new DateTime(2026, 1, 5, 8, 0, 0, 0, DateTimeKind.Unspecified), "Teclado mecánico Redragon Kumara", 189.90m, 25, 10 },
                    { 6, "MOU-001", "Receptor USB, 1000 DPI", new DateTime(2026, 1, 5, 8, 0, 0, 0, DateTimeKind.Unspecified), "Mouse inalámbrico Logitech M185", 59.90m, 40, 10 },
                    { 7, "MOU-002", "Sensor óptico 6400 DPI", new DateTime(2026, 1, 12, 8, 0, 0, 0, DateTimeKind.Unspecified), "Mouse gamer Razer DeathAdder", 219.00m, 6, 5 },
                    { 8, "AUD-001", "Bluetooth, 50 h de batería", new DateTime(2026, 1, 12, 8, 0, 0, 0, DateTimeKind.Unspecified), "Audífonos Sony WH-CH520", 199.00m, 12, 5 },
                    { 9, "IMP-001", "Multifuncional, Wi-Fi, tinta continua", new DateTime(2026, 1, 12, 8, 0, 0, 0, DateTimeKind.Unspecified), "Impresora Epson EcoTank L3250", 899.00m, 1, 2 },
                    { 10, "ALM-001", "NVMe PCIe 4.0", new DateTime(2026, 2, 2, 8, 0, 0, 0, DateTimeKind.Unspecified), "Disco SSD Kingston 1 TB", 279.90m, 20, 8 },
                    { 11, "ALM-002", "USB 3.2", new DateTime(2026, 2, 2, 8, 0, 0, 0, DateTimeKind.Unspecified), "Memoria USB SanDisk 64 GB", 35.90m, 60, 20 },
                    { 12, "RED-001", "Doble banda AC1200", new DateTime(2026, 2, 2, 8, 0, 0, 0, DateTimeKind.Unspecified), "Router TP-Link Archer C6", 169.00m, 9, 4 },
                    { 13, "CAM-001", "Full HD 1080p con micrófono", new DateTime(2026, 2, 16, 8, 0, 0, 0, DateTimeKind.Unspecified), "Webcam Logitech C920", 329.00m, 3, 5 },
                    { 14, "ACC-001", "5 ventiladores, hasta 17\"", new DateTime(2026, 2, 16, 8, 0, 0, 0, DateTimeKind.Unspecified), "Base refrigerante para laptop", 79.90m, 18, 6 },
                    { 15, "ACC-002", "HDMI 4K, 3 USB, lector SD", new DateTime(2026, 2, 16, 8, 0, 0, 0, DateTimeKind.Unspecified), "Hub USB-C 7 en 1", 129.00m, 0, 3 }
                });

            migrationBuilder.InsertData(
                table: "Ventas",
                columns: new[] { "Id", "ClienteId", "Fecha", "Total" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2026, 5, 6, 10, 15, 0, 0, DateTimeKind.Unspecified), 2558.90m },
                    { 2, 2, new DateTime(2026, 5, 21, 16, 40, 0, 0, DateTimeKind.Unspecified), 1289.70m },
                    { 3, 3, new DateTime(2026, 6, 10, 11, 5, 0, 0, DateTimeKind.Unspecified), 306.70m },
                    { 4, 4, new DateTime(2026, 6, 27, 18, 20, 0, 0, DateTimeKind.Unspecified), 3278.90m },
                    { 5, 5, new DateTime(2026, 7, 8, 9, 45, 0, 0, DateTimeKind.Unspecified), 970.80m },
                    { 6, 1, new DateTime(2026, 7, 30, 15, 30, 0, 0, DateTimeKind.Unspecified), 639.70m },
                    { 7, 6, new DateTime(2026, 8, 14, 12, 10, 0, 0, DateTimeKind.Unspecified), 1118.00m },
                    { 8, 7, new DateTime(2026, 9, 3, 17, 0, 0, 0, DateTimeKind.Unspecified), 787.00m },
                    { 9, 8, new DateTime(2026, 9, 25, 10, 30, 0, 0, DateTimeKind.Unspecified), 2808.70m },
                    { 10, 9, new DateTime(2026, 10, 15, 14, 50, 0, 0, DateTimeKind.Unspecified), 748.90m }
                });

            migrationBuilder.InsertData(
                table: "DetalleVentas",
                columns: new[] { "Id", "Cantidad", "PrecioUnitario", "ProductoId", "Subtotal", "VentaId" },
                values: new object[,]
                {
                    { 1, 1, 2499.00m, 1, 2499.00m, 1 },
                    { 2, 1, 59.90m, 6, 59.90m, 1 },
                    { 3, 2, 549.90m, 3, 1099.80m, 2 },
                    { 4, 1, 189.90m, 5, 189.90m, 2 },
                    { 5, 1, 199.00m, 8, 199.00m, 3 },
                    { 6, 3, 35.90m, 11, 107.70m, 3 },
                    { 7, 1, 3199.00m, 2, 3199.00m, 4 },
                    { 8, 1, 79.90m, 14, 79.90m, 4 },
                    { 9, 1, 899.00m, 9, 899.00m, 5 },
                    { 10, 2, 35.90m, 11, 71.80m, 5 },
                    { 11, 2, 279.90m, 10, 559.80m, 6 },
                    { 12, 1, 79.90m, 14, 79.90m, 6 },
                    { 13, 1, 899.00m, 4, 899.00m, 7 },
                    { 14, 1, 219.00m, 7, 219.00m, 7 },
                    { 15, 2, 329.00m, 13, 658.00m, 8 },
                    { 16, 1, 129.00m, 15, 129.00m, 8 },
                    { 17, 1, 2499.00m, 1, 2499.00m, 9 },
                    { 18, 1, 189.90m, 5, 189.90m, 9 },
                    { 19, 2, 59.90m, 6, 119.80m, 9 },
                    { 20, 1, 549.90m, 3, 549.90m, 10 },
                    { 21, 1, 199.00m, 8, 199.00m, 10 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_Correo",
                table: "Clientes",
                column: "Correo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_Documento",
                table: "Clientes",
                column: "Documento",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DetalleVentas_ProductoId",
                table: "DetalleVentas",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleVentas_VentaId",
                table: "DetalleVentas",
                column: "VentaId");

            migrationBuilder.CreateIndex(
                name: "IX_Productos_Codigo",
                table: "Productos",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ventas_ClienteId",
                table: "Ventas",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Ventas_Fecha",
                table: "Ventas",
                column: "Fecha");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DetalleVentas");

            migrationBuilder.DropTable(
                name: "Productos");

            migrationBuilder.DropTable(
                name: "Ventas");

            migrationBuilder.DropTable(
                name: "Clientes");
        }
    }
}
