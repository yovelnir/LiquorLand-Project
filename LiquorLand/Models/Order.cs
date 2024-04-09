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
        public DbSet<Order> orders { get; set; }

        public OrderContext(DbContextOptions<OrderContext> options)
        : base(options)
        {
        }

        public OrderContext()
        {
        }

/*        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //Default values for orderItems and userId Amount

            builder.Entity<Order>()
                .Property(p => p.orderItems).HasDefaultValue(null).IsRequired(false);
            builder.Entity<Order>()
                .Property(p => p.userId).IsRequired(true);
            builder.Entity<Order>()
                .Property(p => p.totalPrice).HasDefaultValue(0.0m).IsRequired(false);
            builder.Entity<Order>()
                .Property(p => p.OrderId).HasDefaultValue(0).IsRequired(false);
        }*/
    }

    public class Order
    {
        [Key]
        [Required]
        public string OrderId { get; set;}

        [Required]
        public string orderItems { get; set; }

        [Required]
        public DateTime orderDate { get; set; }

        public string? userId { get; set; }

        [Required]
        public decimal? totalPrice {  get; set; }

    }
}
