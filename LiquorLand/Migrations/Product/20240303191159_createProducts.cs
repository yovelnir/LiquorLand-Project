using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiquorLand.Migrations.Product
{
    public partial class createProducts : Migration
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
                    ProductLocation = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ProductPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ProductSubCategory = table.Column<string>(type: "nvarchar(max)", maxLength: 100, nullable: true),
                    ProductDescription = table.Column<string>(type: "nvarchar(max)", maxLength: 100, nullable: true),
                    ProductImage = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
