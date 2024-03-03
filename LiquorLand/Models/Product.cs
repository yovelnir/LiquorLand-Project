using LiquorLand.Data;
using Microsoft.EntityFrameworkCore;
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
        [RegularExpression("^[a-zA-Z0-9]+$")]
        [StringLength(5, MinimumLength = 5, ErrorMessage = "Serial must be exactly 5 characters long")]
        public string Serial { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Product name must be atleast 2 characters long")]
        public string ProductName { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Product category must be atleast 2 characters long")]
        public string ProductCategory { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(5)")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Product location must be atleast 2 characters long")]
        public string ProductLocation { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18, 2)")]
        [RegularExpression("^[0-9.]+$", ErrorMessage = "Price can only be a numerical value!")]
        public decimal ProductPrice { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        [StringLength(100)]
        public string ProductSubCategory { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        [StringLength(100)]
        public string ProductDescription { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string ProductImage { get; set; }

        public async Task<string> SaveImage(IFormFile image)
        {
            // Generate a unique file name
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);

            // Get the upload directory path (customize based on your application)
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
    }
}
