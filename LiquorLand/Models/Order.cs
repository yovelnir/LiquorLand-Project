using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections;
using System.Configuration;
using System.Reflection.Emit;

namespace LiquorLand.Models
{

    public class OrderContext : DbContext
    {
        public DbSet<Order>? orders { get; set; }

        public OrderContext(DbContextOptions<OrderContext> options)
        : base(options)
        {
        }

        public OrderContext()
        {
        }
    }

    public class Order
    {
        [Key]
        [Required]
        public string? OrderId { get; set;}

        [Required]
        public string? orderItems { get; set; }

        [Required]
        public DateTime orderDate { get; set; }

        public string? userId { get; set; }

        [Required]
        public decimal? totalPrice {  get; set; }

    }
}
