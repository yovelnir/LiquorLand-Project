using Microsoft.AspNetCore.Mvc;
using Braintree;
using System;
using System.Collections.Generic;
using System.Linq;
using LiquorLand.Models;
using Newtonsoft.Json;
using Braintree.Exceptions;
using LiquorLand.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using LiquorLand.Migrations.Product;
using Product = LiquorLand.Models.Product;

namespace LiquorLand.Controllers
{
    public class BrainTreeController : Controller
    {
        public IBraintreeGate _brain { get; set; }
        private readonly OrderContext _orderContext;
        private readonly UserManager<Users> _userManager;
        private readonly ProductContext _productContext;

        public BrainTreeController(IBraintreeGate brain, OrderContext orderContext, UserManager<Users> userManager, ProductContext productContext)
        {
            _brain = brain;
            _orderContext = orderContext;
            _userManager = userManager;
            _productContext = productContext;
        }
        public IActionResult GenerateToken()
        {
            var gateway = _brain.GetGateway();
            var clientToken = gateway.ClientToken.Generate();
            ViewBag.ClientToken = clientToken;

            return View();
        }

        public ShoppingCart httpCart()
        {
            var shoppingCartString = HttpContext.Session.GetString("cart");
            ShoppingCart? shoppingCart = null;

            if (shoppingCartString != null)
            {
                shoppingCart = JsonConvert.DeserializeObject<ShoppingCart>(shoppingCartString);
                HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(shoppingCart));
            }
            return shoppingCart;
        }


        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> GenerateToken(IFormCollection collections)
        {

            decimal amount = 0;
            ShoppingCart? shoppingCart = httpCart();

            if (shoppingCart != null)
            {
                amount = shoppingCart.GetTotal();

                foreach (cartsItem item in shoppingCart.CartItems)
                {
                    Product? p = await _productContext.Products.FindAsync(item.cartItem.Serial);
                    if (p != null)
                        if (p.Stock < item.Quantity)
                        {
                            string? OrderFail;
                            if (p.Stock == 0)
                            {
                                shoppingCart.CartItems.Remove(item);
                                OrderFail = $"{p.ProductName} not enough stock to proceed with order, the product is out of stock, removed the item from the cart.";
                            }
                            else
                            {
                                item.Quantity = (int)p.Stock;
                                OrderFail = $"{p.ProductName} not enough stock to proceed with order, current stock is {p.Stock}, quantity has been updated.";
                            }
                            HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(shoppingCart));

                            return RedirectToAction("shoppingCarts", "ShoppingCart", new { OrderFail = OrderFail });
                        }
                }
            }

            //ShoppingCart totalFee;
            string nonceFormClient = collections["payment_method_nonce"];
            string firstName = collections["first_name"];
            string lastname = collections["last_name"];
            string country = collections["country"];
            string address = collections["address"];
            string email = collections["Email"];
            string zip = collections["postal_code"];
            string phone = collections["phone"];


            var request = new TransactionRequest
            {
                Amount = amount,
                PaymentMethodNonce = nonceFormClient,
                Customer = new CustomerRequest
                {
                    FirstName = firstName,
                    LastName = lastname,
                    Phone = phone,
                    Email = email,

                },
                BillingAddress = new AddressRequest
                {
                    FirstName = firstName,
                    LastName = lastname,
                    CountryName = country,
                    StreetAddress = address,
                    PostalCode = zip,
                },
                ShippingAddress = new AddressRequest
                {
                    FirstName = firstName,
                    LastName = lastname,
                    CountryName = country,
                    StreetAddress = address,
                    PostalCode = zip,
                },
                Options = new TransactionOptionsRequest
                {
                    SubmitForSettlement = true
                },

            };

            var gateway = _brain.GetGateway();
            Result<Transaction> result = gateway.Transaction.Sale(request);
            if (result.Target.ProcessorResponseText == "Approved")
            {
                return RedirectToAction("AddOrder");
            }
            return RedirectToAction("GenerateToken");
        }

        public async Task<IActionResult> AddOrder(bool semaphoreRelease = false)
        {
            var user = await _userManager.GetUserAsync(User);
            ShoppingCart? shoppingCart = httpCart();
            Order? order = new Order();
            order.OrderId = (_orderContext.orders.Count<Order>() > 0 ? _orderContext.orders.Count<Order>() + 1 : 1).ToString();
            order.totalPrice = shoppingCart.GetTotal();
            order.orderDate = DateTime.Now;
            if (user != null)
            {
                order.userId = user.Id;
            }

            Dictionary<string, int> orderedItems = new Dictionary<string, int>();
            foreach (cartsItem item in shoppingCart.CartItems)
            {
                orderedItems.Add(item.cartItem.Serial, item.Quantity);
            }
            order.orderItems = JsonConvert.SerializeObject(orderedItems);
            await _orderContext.orders.AddAsync(order);
            _orderContext.SaveChanges();

            return RedirectToAction("removeCart", "ShoppingCart", order);
        }

        public IActionResult successPayment(Order order)
        {
            List<cartsItem> Items = new List<cartsItem>();
            Product product;
            Dictionary<string, int> orders = JsonConvert.DeserializeObject<Dictionary<string, int>>(order.orderItems);

            foreach (KeyValuePair<string, int> entry in orders)
            {
                product = _productContext.Products.Find(entry.Key);
                Items.Add(new cartsItem(product));
                Items.Find(item => item.cartItem.Equals(product)).Quantity = entry.Value;
            }

            ViewBag.totalPrice = order.totalPrice;
            ViewBag.orderNumber = order.OrderId;
            return View("successPayment", Items);
        }
    }
}
