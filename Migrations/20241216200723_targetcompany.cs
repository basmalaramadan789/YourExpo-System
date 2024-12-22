using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YourExpo.Migrations
{
    /// <inheritdoc />
    public partial class targetcompany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductTargetCountry_Products_ProductId",
                table: "ProductTargetCountry");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductTargetCountry_TargetCountry_TargetCountryId",
                table: "ProductTargetCountry");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TargetCountry",
                table: "TargetCountry");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductTargetCountry",
                table: "ProductTargetCountry");

            migrationBuilder.RenameTable(
                name: "TargetCountry",
                newName: "TargetCountries");

            migrationBuilder.RenameTable(
                name: "ProductTargetCountry",
                newName: "ProductTargetCountries");

            migrationBuilder.RenameIndex(
                name: "IX_ProductTargetCountry_TargetCountryId",
                table: "ProductTargetCountries",
                newName: "IX_ProductTargetCountries_TargetCountryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TargetCountries",
                table: "TargetCountries",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductTargetCountries",
                table: "ProductTargetCountries",
                columns: new[] { "ProductId", "TargetCountryId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ProductTargetCountries_Products_ProductId",
                table: "ProductTargetCountries",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductTargetCountries_TargetCountries_TargetCountryId",
                table: "ProductTargetCountries",
                column: "TargetCountryId",
                principalTable: "TargetCountries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductTargetCountries_Products_ProductId",
                table: "ProductTargetCountries");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductTargetCountries_TargetCountries_TargetCountryId",
                table: "ProductTargetCountries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TargetCountries",
                table: "TargetCountries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductTargetCountries",
                table: "ProductTargetCountries");

            migrationBuilder.RenameTable(
                name: "TargetCountries",
                newName: "TargetCountry");

            migrationBuilder.RenameTable(
                name: "ProductTargetCountries",
                newName: "ProductTargetCountry");

            migrationBuilder.RenameIndex(
                name: "IX_ProductTargetCountries_TargetCountryId",
                table: "ProductTargetCountry",
                newName: "IX_ProductTargetCountry_TargetCountryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TargetCountry",
                table: "TargetCountry",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductTargetCountry",
                table: "ProductTargetCountry",
                columns: new[] { "ProductId", "TargetCountryId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ProductTargetCountry_Products_ProductId",
                table: "ProductTargetCountry",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductTargetCountry_TargetCountry_TargetCountryId",
                table: "ProductTargetCountry",
                column: "TargetCountryId",
                principalTable: "TargetCountry",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
