using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace StoreAPI.Migrations
{
    /// <inheritdoc />
    public partial class Seed_Store_Products : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Store",
                columns: new[] { "Id", "Description", "Latitude", "Longitude" },
                values: new object[,]
                {
                    { 1, "Plaza Mayor León", 21.154, -101.69459999999999 },
                    { 2, "Centro Max", 21.094799999999999, -101.6417 },
                    { 3, "Plaza Galerías Las Torres", 21.121099999999998, -101.6613 },
                    { 4, "Outlet Mulza", 21.0459, -101.58620000000001 },
                    { 5, "La Gran Plaza León", 21.128, -101.6827 },
                    { 6, "Altacia", 21.128, -102.0 }
                });

            migrationBuilder.InsertData(
                table: "Product",
                columns: new[] { "Id", "Description", "Name", "Price", "StoreId" },
                values: new object[,]
                {
                    { 1, "Calzado típico de León", "Zapatos de piel", 1200.0, 4 },
                    { 2, "Artesanía local", "Bolsa de cuero", 950.0, 4 },
                    { 3, "Comida rápida", "Hamburguesa doble", 120.0, 1 },
                    { 4, "Especialidad italiana", "Pizza familiar", 220.0, 2 },
                    { 5, "Taza grande", "Café americano", 45.0, 3 },
                    { 6, "Bebida refrescante", "Refresco 600ml", 20.0, 5 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Store",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Store",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Store",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Store",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Store",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Store",
                keyColumn: "Id",
                keyValue: 5);
        }
    }
}
