using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections;
using System.Configuration;
using System.Reflection.Emit;

namespace LiquorLand.Models
{

    public class notifyContext : DbContext
    {
        public DbSet<notification> notify { get; set; }

        public notifyContext(DbContextOptions<notifyContext> options)
        : base(options)
        {
        }

        public notifyContext()
        {
        }

    }
    public class notification
    {
        [Key]
        [Required]
        public string? productId { get; set; }

        [Required]
        public string? userId { get; set; }

        [Required]
        public Boolean BackInStock {  get; set; }

    }
}
