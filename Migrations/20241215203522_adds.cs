using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YourExpo.Migrations
{
    /// <inheritdoc />
    public partial class adds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Supplier_AspNetUsers_ApplicationUserId",
                table: "Supplier");

            migrationBuilder.DropIndex(
                name: "IX_Supplier_ApplicationUserId",
                table: "Supplier");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Supplier");

            migrationBuilder.AddColumn<int>(
                name: "SupplierId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_SupplierId",
                table: "AspNetUsers",
                column: "SupplierId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Supplier_SupplierId",
                table: "AspNetUsers",
                column: "SupplierId",
                principalTable: "Supplier",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Supplier_SupplierId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_SupplierId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "SupplierId",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Supplier",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Supplier_ApplicationUserId",
                table: "Supplier",
                column: "ApplicationUserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Supplier_AspNetUsers_ApplicationUserId",
                table: "Supplier",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
