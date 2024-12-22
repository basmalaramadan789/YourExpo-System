using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace YourExpo.Migrations
{
    /// <inheritdoc />
    public partial class seedingsuplier : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Supplier",
                columns: new[] { "Id", "ContactInfo", "IsApproved", "SupplierName" },
                values: new object[,]
                {
                    { 1, "contact1@supplier.com", true, "Supplier 1" },
                    { 2, "contact2@supplier.com", false, "Supplier 2" },
                    { 3, "contact3@supplier.com", true, "Supplier 3" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Supplier",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Supplier",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Supplier",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
