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

    }
}
