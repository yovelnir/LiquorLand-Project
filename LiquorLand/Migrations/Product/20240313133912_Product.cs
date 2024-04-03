using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiquorLand.Migrations.Product
{
    public partial class Product : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Serial = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(max)", maxLength: 100, nullable: false),
                    ProductCategory = table.Column<string>(type: "nvarchar(max)", maxLength: 100, nullable: false),
                    ProductSubCategory = table.Column<string>(type: "nvarchar(max)", maxLength: 100, nullable: false),
                    AlcoholPercentage = table.Column<decimal>(type: "decimal(18,1)", nullable: false),
                    Volume = table.Column<int>(type: "int", nullable: false),
                    ProductPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ProductMaker = table.Column<string>(type: "nvarchar(max)", maxLength: 100, nullable: true),
                    Country = table.Column<string>(type: "nvarchar(max)", maxLength: 100, nullable: true),
                    Stock = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    ProductDescription = table.Column<string>(type: "nvarchar(max)", maxLength: 100, nullable: true),
                    ProductImage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SaleAmount = table.Column<int>(type: "int", nullable: true, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Serial);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
