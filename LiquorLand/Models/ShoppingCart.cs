using Microsoft.AspNetCore.Mvc;
using LiquorLand.Controllers;
using LiquorLand.ViewModels;
using Newtonsoft.Json;

namespace LiquorLand.Models
{
    public class ShoppingCart
    {       
        public int Count { get; set; }
       
        
        public List<Product> CartItems = new List<Product>();
        public HashSet<Product> UniqueProduct { get; set; }


        public void AddToCart(Product product)
        {
            CartItems.Add(product);
        }

    
        public bool InCat(Product product)
        {
            return CartItems.Any(x=> x.Serial == product.Serial);
        }
    
        public void RemoveFromCart(Product s)
        {
            CartItems.Remove(s);
        }

        public List<Product> GetCartItems()
        {
            return CartItems.ToList();
        }

        public decimal GetTotal()
        {
            decimal? total = (from cartItem in CartItems select cartItem.ProductPrice).Sum();
            return total ?? decimal.Zero;
        }

        public int numProduct(string serial)
        {
            int count = 0;
            foreach (var item in CartItems)
            {
                if (item.Serial == serial)
                    count += 1;
            }
            return count;
        }


     


    }


   
}
