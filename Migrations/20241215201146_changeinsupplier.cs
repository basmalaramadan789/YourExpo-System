using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YourExpo.Migrations
{
    /// <inheritdoc />
    public partial class changeinsupplier : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Supplier");

            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "Supplier",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "Supplier");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Supplier",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
