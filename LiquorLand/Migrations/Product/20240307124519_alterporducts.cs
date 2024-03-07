using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiquorLand.Migrations.Product
{
    public partial class alterporducts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductLocation",
                table: "Products");

            migrationBuilder.AddColumn<decimal>(
                name: "Stock",
                table: "Products",
                type: "decimal(18,0)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Stock",
                table: "Products");

            migrationBuilder.AddColumn<string>(
                name: "ProductLocation",
                table: "Products",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }
    }
}
