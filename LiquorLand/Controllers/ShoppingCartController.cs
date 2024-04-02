using LiquorLand.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using System.Drawing.Imaging;
using System.Drawing.Printing;

namespace LiquorLand.Controllers
{
    public class ShoppingCartController : Controller
    {
        private ShoppingCart shoppingCart;
        public readonly ProductContext _productContext;

        public ShoppingCartController(ProductContext productContext)
        {
            _productContext = productContext;
            shoppingCart = new ShoppingCart();
        }

        [HttpGet]
        public IActionResult addToCart(string serial, int quantity) 
        {
            var shoppingCartString = HttpContext.Session.GetString("cart");
            ShoppingCart? shoppingCart = null;
            Product? p = _productContext.Products.Find(serial);

            if (shoppingCartString != null)
                shoppingCart = JsonConvert.DeserializeObject<ShoppingCart>(shoppingCartString);
            else
                shoppingCart = new ShoppingCart();

            if (shoppingCart != null && p != null)
            {
                cartsItem? c = shoppingCart.CartItems.Find(item => item.cartItem.Serial == serial);

                if (c != null)
                {
                    if (p.Stock > c.Quantity)
                    {
                        shoppingCart.AddToCart(c.cartItem, quantity); 
                        HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(shoppingCart));
                        return Json(new { success = true });
                    }
                    else
                    {
                        return Json(new { success = false });
                    }
                }
                else
                {
                    shoppingCart.AddToCart(p, quantity); 
                    HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(shoppingCart));
                    return Json(new { success = true });
                }
            }
            return Json(new { success = false });
        }
        public int getNumPerItemInCart(string serial)
        {
            var shoppingCartString = HttpContext.Session.GetString("cart");
            ShoppingCart? shoppingCart = null;

            if (shoppingCartString != null)
                shoppingCart = JsonConvert.DeserializeObject<ShoppingCart>(shoppingCartString);

            if (shoppingCart != null)
            {
                cartsItem? c = shoppingCart.CartItems.Find(item => item.cartItem.Serial == serial);
                if (c != null)
                {
                    return c.Quantity;
                }
            }
            return 0;
        }


        public int CountItems()
        {
            var shoppingCartString = HttpContext.Session.GetString("cart");
            ShoppingCart? shoppingCart = null;

            if (shoppingCartString != null)
                shoppingCart = JsonConvert.DeserializeObject<ShoppingCart>(shoppingCartString);

            if (shoppingCart != null)
                return shoppingCart.CartItems.Sum(items => items.Quantity);
            return 0;
        }


        public IActionResult plusItem(string serial, bool action)
        {
            var shoppingCartString = HttpContext.Session.GetString("cart");
            ShoppingCart? shoppingCart = null;
            Product? p = _productContext.Products.Find(serial);
            if (shoppingCartString != null)
                shoppingCart = JsonConvert.DeserializeObject<ShoppingCart>(shoppingCartString);

            if (shoppingCart != null)
            {
                cartsItem? c = shoppingCart.CartItems.Find(item => item.cartItem.Serial == serial);

                if (c != null && p != null)
                {
                    if (action)
                    {
                        if (p.Stock > c.Quantity)
                        {
                            shoppingCart.ClickPlus(c.cartItem);
                            HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(shoppingCart));
                        }
                        else
                        {
                            return Json(new { success = false });
                        }
                    }
                    else
                    { 
                        if (p.Stock >= c.Quantity)
                        {
                            shoppingCart.ClickMinus(c.cartItem);
                            HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(shoppingCart));
                        }
                    }
                   
                }
            }
            return shoppingCarts(true);
        }
        public IActionResult shoppingCarts(bool partial = false)
        {
            var shoppingCartString = HttpContext.Session.GetString("cart");
            ShoppingCart? shoppingCart = null;

            if (shoppingCartString != null)
            {
                shoppingCart = JsonConvert.DeserializeObject<ShoppingCart>(shoppingCartString);
            }
            else
                shoppingCart = new ShoppingCart();

            if (partial)
                return PartialView("_PartialCart", shoppingCart);
            return View("shoppingCart", shoppingCart);
        }

        public IActionResult RemoveItem(string serial)
        {
            var shoppingCartString = HttpContext.Session.GetString("cart");
            ShoppingCart? shoppingCart = null;


            if (shoppingCartString != null)
            {
                shoppingCart = JsonConvert.DeserializeObject<ShoppingCart>(shoppingCartString);

                if (shoppingCart != null)
                {
                    cartsItem? c = shoppingCart.CartItems.Find(item => item.cartItem.Serial == serial);
                    if (c != null)
                    {
                        shoppingCart.RemoveFromCart(c.cartItem);
                        HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(shoppingCart));
                    }
                }
            }
            return shoppingCarts(true);
        }

    }
}

