using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiquorLand.Migrations.Product
{
    public partial class products2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Stock",
                table: "Products",
                type: "int",
                nullable: true,
                defaultValue: 0,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,0)");

            migrationBuilder.AlterColumn<string>(
                name: "ProductSubCategory",
                table: "Products",
                type: "nvarchar(max)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "AlcoholPercentage",
                table: "Products",
                type: "decimal(3,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Products",
                type: "nvarchar(max)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProductMaker",
                table: "Products",
                type: "nvarchar(max)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SaleAmount",
                table: "Products",
                type: "int",
                nullable: true,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Volume",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AlcoholPercentage",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ProductMaker",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "SaleAmount",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Volume",
                table: "Products");

            migrationBuilder.AlterColumn<decimal>(
                name: "Stock",
                table: "Products",
                type: "decimal(18,0)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldDefaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "ProductSubCategory",
                table: "Products",
                type: "nvarchar(max)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldMaxLength: 100);
        }
    }
}
