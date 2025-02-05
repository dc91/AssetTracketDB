using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AssetTracketDB.Migrations
{
    /// <inheritdoc />
    public partial class AddingSeedData2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Offices",
                columns: new[] { "Id", "Currency", "Name" },
                values: new object[,]
                {
                    { 1, "SEK", "SWE" },
                    { 2, "USD", "USA" },
                    { 3, "EUR", "FIN" }
                });

            migrationBuilder.InsertData(
                table: "Asset",
                columns: new[] { "Id", "Currency", "Discriminator", "Manufacturer", "ModelName", "OfficeId", "Price", "PurchaseDate" },
                values: new object[,]
                {
                    { 1, "SEK", "Laptop", "Apple", "MacBook", 1, 19990m, new DateOnly(2021, 3, 9) },
                    { 2, "SEK", "Laptop", "Apple", "MacBook Pro", 1, 21500m, new DateOnly(2021, 7, 29) },
                    { 3, "SEK", "Laptop", "Apple", "MacBook Air", 1, 20899m, new DateOnly(2023, 7, 15) },
                    { 4, "USD", "Laptop", "Asus", "Vivobook", 2, 1899m, new DateOnly(2023, 12, 23) },
                    { 5, "USD", "Laptop", "Asus", "Zenbook", 2, 1299m, new DateOnly(2023, 12, 21) },
                    { 6, "USD", "Laptop", "Asus", "ROG", 2, 2099m, new DateOnly(2023, 8, 4) },
                    { 7, "EUR", "Laptop", "Apple", "MacBook Pro", 3, 1900m, new DateOnly(2021, 3, 21) },
                    { 8, "EUR", "Laptop", "HP", "Spectre", 3, 1300m, new DateOnly(2023, 4, 13) },
                    { 9, "EUR", "Laptop", "Dell", "Inspiron", 3, 1100m, new DateOnly(2023, 5, 9) },
                    { 10, "SEK", "Mobile", "Apple", "iPhone 14", 1, 12990m, new DateOnly(2023, 7, 2) },
                    { 11, "SEK", "Mobile", "Apple", "iPhone 13", 1, 11490m, new DateOnly(2023, 10, 11) },
                    { 12, "SEK", "Mobile", "Samsung", "Galaxy S22", 1, 9990m, new DateOnly(2023, 8, 3) },
                    { 13, "USD", "Mobile", "Apple", "iPhone 14 Pro", 2, 999m, new DateOnly(2023, 9, 15) },
                    { 14, "USD", "Mobile", "Samsung", "Galaxy S22", 2, 799m, new DateOnly(2022, 4, 17) },
                    { 15, "USD", "Mobile", "Google", "Pixel 7", 2, 699m, new DateOnly(2022, 9, 23) },
                    { 16, "EUR", "Mobile", "Apple", "iPhone 14", 3, 1100m, new DateOnly(2023, 1, 25) },
                    { 17, "EUR", "Mobile", "Samsung", "Galaxy S22", 3, 900m, new DateOnly(2021, 8, 21) },
                    { 18, "EUR", "Mobile", "Google", "Pixel 7", 3, 800m, new DateOnly(2021, 3, 31) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Asset",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Asset",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Asset",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Asset",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Asset",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Asset",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Asset",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Asset",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Asset",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Asset",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Asset",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Asset",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Asset",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Asset",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Asset",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Asset",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Asset",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Asset",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Offices",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Offices",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Offices",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
