using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiquorLand.Migrations.Product
{
    public partial class Products : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Serial = table.Column<string>(type: "nvarchar(500)", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductCategory = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductLocation = table.Column<string>(type: "nvarchar(5)", nullable: false),
                    ProductPrice = table.Column<float>(type: "real", nullable: false),
                    ProductSubCategory = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductImage = table.Column<byte[]>(type: "VARBINARY(max)", nullable: true)
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
