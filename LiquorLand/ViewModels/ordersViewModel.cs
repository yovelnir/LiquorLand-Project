using LiquorLand.Models;
using LiquorLand.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;

namespace LiquorLand.ViewModels
{
    public class ordersViewModel
    {
        public List<Order> orders = new List<Order>();
        public int TotalOrders { get; set; }
        public string? userId { get; set; }
    }
}
