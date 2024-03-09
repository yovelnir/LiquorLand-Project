using LiquorLand.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;

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

        public ProductContext()
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //Default values for Stock and Sales Amount
            builder.Entity<Product>()
                .Property(p => p.SaleAmount).HasDefaultValue(0).IsRequired(false);
            builder.Entity<Product>()
                .Property(p => p.Stock).HasDefaultValue(0).IsRequired(false);
        }
    }


    public class Product
    {
        /*Product Serial*/
        [Key]
        [Required]
        [Column(TypeName = "nvarchar(500)")]
        [RegularExpression("^[a-zA-Z0-9]+$")]
        [StringLength(5, MinimumLength = 5, ErrorMessage = "Serial must be exactly 5 characters long")]
        public string Serial { get; set; }

        /*Product Name*/
        [Required]
        [Column(TypeName = "nvarchar(max)")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Product name must be atleast 2 characters long")]
        public string ProductName { get; set; }

        /*Product Category: Wine, Beer, etc...*/
        [Required]
        [Column(TypeName = "nvarchar(max)")]
        [StringLength(100)]
        public string ProductCategory { get; set; }

        /*Product Sub Category: Red, White, Wheat, etc...*/
        [Required]
        [Column(TypeName = "nvarchar(max)")]
        [StringLength(100)]
        public string ProductSubCategory { get; set; }

        /*Product Alcohol Precentage*/
        [Required]
        [Column(TypeName = "decimal(18,1)")]
        public decimal AlcoholPercentage { get; set; }

        /*Product Liquid Volume*/
        [Required]
        [Column(TypeName = "int")]
        public int Volume { get; set; }

        /*Product Price*/
        [Required]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18, 2)")]
        [RegularExpression("^[0-9.]+$", ErrorMessage = "Price can only be a numerical value!")]
        public decimal ProductPrice { get; set; }

        /*Product Maker*/
        [Column(TypeName = "nvarchar(max)")]
        [StringLength(100)]
        public string ProductMaker { get; set; }

        /*Product Origin Country*/
        [Column(TypeName = "nvarchar(max)")]
        [StringLength(100)]
        public string Country { get; set; }

        /*Product Stock*/
        [Column(TypeName = "int")]
        [RegularExpression("^[0-9]+$", ErrorMessage = "Stock can only be a numerical value!")]
        public int? Stock { get; set; }

        /*Product Description*/
        [Column(TypeName = "nvarchar(max)")]
        [StringLength(100)]
        public string ProductDescription { get; set; }

        /*Product Image Path*/
        [Column(TypeName = "nvarchar(max)")]
        public string ProductImage { get; set; }

        /*Product Sales Amount*/
        [Column(TypeName = "int")]
        public int? SaleAmount { get; set; }




        public async Task<string> SaveImage(IFormFile image)
        {
            // Generate a unique file name
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);

            // Get the upload directory path
            string uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "productImages");

            // Create the directory if it doesn't exist
            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
            }

            // Save the uploaded image to the specified path
            await using (FileStream fileStream = new FileStream(Path.Combine(uploadsPath, fileName), FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }

            // Return the saved file name
            return Path.Combine("\\productImages", fileName);
        }

        public void RemoveImage(string path)
        {
            string deletePath = $"{Directory.GetCurrentDirectory()}\\wwwroot\\{path}";

            if(File.Exists(deletePath))
                File.Delete(deletePath);
        }
    }
}
