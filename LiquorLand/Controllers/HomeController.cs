using LiquorLand.Models;
using LiquorLand.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Evaluation;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;
using NuGet.Protocol;
using System.Diagnostics;


namespace LiquorLand.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ProductContext _productContext;
        private ShoppingCart cart = new ShoppingCart();

        

        public HomeController(ILogger<HomeController> logger, ProductContext productContext)
        {
            _logger = logger;
            _productContext = productContext;
        }

        public IActionResult Index()
        {
            Random rand = new Random();
            int num = rand.Next(1, 6);
            ViewBag.bg = $"\\bg\\vid{num}.mp4";
            ViewBag.bgi = $"\\bg\\vid{num}.jpg";


            productViewModel products = new productViewModel();
            products.all_products = _productContext.Products.ToList<Product>();

            products.hot_products = products.getHotProducts(_productContext);

            return View(products);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Privacy()
        {
            return View();
        }

        [Route("AccessDenied")]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }



        public IActionResult ViewCart()
        {
            //var cart = shoppingCarts.Get
            return View();
        }


        public object addToCart(string serial)
        {
            // Retrieve ShoppingCart from session
            var shoppingCartString = HttpContext.Session.GetString("cart");
            ShoppingCart? shoppingCart = null;
            if (shoppingCartString != null)
                shoppingCart = JsonConvert.DeserializeObject<ShoppingCart>(shoppingCartString);
            else
                shoppingCart = new ShoppingCart();

            if (shoppingCart != null)
            {
                Product? product = _productContext.Products.Find(serial);

                if (product != null)

                    HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(shoppingCart));

            }
            return RedirectToAction("Index", "home");
        }

        public IActionResult shoppingCarts()
        {

            var shoppingCartString = HttpContext.Session.GetString("cart");
            ShoppingCart shoppingCart = null;
            if (shoppingCartString != null)
            {
                shoppingCart = JsonConvert.DeserializeObject<ShoppingCart>(shoppingCartString);
    
            }
            else
                shoppingCart = new ShoppingCart();

   
            return View("shoppingCart", shoppingCart);
        }

        public IActionResult updateProduct(string serial)
        {
            
            return View("shoppingCart");

        }

        public IActionResult addProduct(string serial) 
        {
            addToCart(serial);
            
            return RedirectToAction("ShoppingCart", "home");
        }


        public IActionResult removeProduct(string serial) 
        {
            var shoppingCartString = HttpContext.Session.GetString("cart");
            ShoppingCart? shoppingCart = null;
            if (shoppingCartString != null)
                shoppingCart = JsonConvert.DeserializeObject<ShoppingCart>(shoppingCartString);


            return View();
        }

     

    }
}
