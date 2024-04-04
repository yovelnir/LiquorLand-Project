using Microsoft.AspNetCore.Mvc;
using LiquorLand.Controllers;
using LiquorLand.ViewModels;
using Newtonsoft.Json;
using System.Timers;

namespace LiquorLand.Models
{
    public class ShoppingCart
    {
        public List<cartsItem> CartItems = new List<cartsItem>();

        public void AddToCart(Product product, int quantity) 
        {
            var existingCartItem = CartItems.Find(item => item.cartItem.Serial == product.Serial);
            if (existingCartItem != null)
            {
                existingCartItem.Quantity += quantity;
            }
            else
            {
                CartItems.Add(new cartsItem(product));
                CartItems.Find(item => item.cartItem.Equals(product)).Quantity = quantity;
            }
        }
        public void ClickPlus(Product product)
        {
            var existingCartItem = CartItems.Find(item => item.cartItem.Serial == product.Serial);
            if (existingCartItem != null)
            {
                existingCartItem.Quantity++;
            }
        }
        public void ClickMinus(Product product)
        {
            var existingCartItem = CartItems.Find(item => item.cartItem.Serial == product.Serial);
            if (existingCartItem != null)
            {
                if(existingCartItem.Quantity > 1) 
                {
                    existingCartItem.Quantity--;
                }
                else
                {
                    CartItems.Remove(existingCartItem);
                }
                
            }
        }

        public void RemoveFromCart(Product product)
        {
            var existingCartItem = CartItems.Find(item => item.cartItem.Serial == product.Serial);
            if (existingCartItem != null)
            {

                CartItems.Remove(existingCartItem);

            }
        }

        public List<cartsItem> GetCartItems()
        {
            return CartItems.ToList();
        }

        public decimal GetTotal()
        {
            decimal? total = 0;
            foreach (cartsItem item in CartItems)
            {
                if (item.Quantity > 1)
                {
                    total += item.Quantity * item.cartItem.ProductPrice;
                }
                else
                    total += item.cartItem.ProductPrice;
            }
            return total ?? decimal.Zero;
        }


    }

    public class cartsItem
    {
        public Product cartItem { get; set; }
        public int Quantity { get; set; }

        public cartsItem(Product p)
        {
            cartItem = p;
            Quantity = 1;
        }


    }
}
