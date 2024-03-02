using LiquorLand.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable
namespace LiquorLand.Models
{
    public class ProductContext : DbContext
    {
        public DbSet<Product> Products { get; set;}

        public ProductContext(DbContextOptions<ProductContext> options)
        : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }


    public class Product
    {
        [Key]
        [Required]
        [Column(TypeName = "nvarchar(500)")]
        public string Serial { get; set; }
        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string ProductName { get; set; }
        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string ProductCategory { get; set; }
        [Required]
        [Column(TypeName = "nvarchar(5)")]
        public string ProductLocation { get; set; }
        [Required]
        public float ProductPrice { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string ProductSubCategory { get; set; }
        [Column(TypeName = "nvarchar(max)")]
        public string ProductDescription { get; set; }
        [Column(TypeName = "VARBINARY(max)")]
        public byte[] ProductImage { get; set; }
    }
}
