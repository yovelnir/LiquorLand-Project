using LiquorLand.Areas.Identity.Data;
using LiquorLand.Migrations.Order;
using LiquorLand.Models;
using LiquorLand.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
        private readonly notifyContext _notifyContext;
        private readonly UserManager<Users> _userManager;
        List<string> prodList;

        public HomeController(ILogger<HomeController> logger, ProductContext productContext, UserManager<Users> userManager, notifyContext notifyContext)
        {
            _logger = logger;
            _productContext = productContext;
            _userManager = userManager;
            _notifyContext = notifyContext;
        }

        public IActionResult Index()
        {
            Random rand = new Random();
            int num = rand.Next(1, 6);
            ViewBag.bg = $"\\bg\\vid{num}.mp4";
            ViewBag.bgi = $"\\bg\\vid{num}.jpg";


            productViewModel products = new productViewModel();
            products.all_products = _productContext.Products.ToList<Product>();

            /*
            //for notification
            prodList = new List<string>(); 
            foreach(notification noti in _notifyContext.notify)
            {
                if (noti.BackInStock)
                {
                    prodList.Add(noti.productId.ToString() + " ");
                }
            }*/
            //ViewData["prList"] = prodList;

            products.hot_products = products.getHotProducts(_productContext);

            return View(products);
        }
        
        public IActionResult OpenModelPopup()
        {
            ViewBag.flag = false;
            productViewModel products = new productViewModel();
            products.all_products = _productContext.Products.ToList<Product>();
            List<Product> proList = new List<Product>();
            prodList = new List<string>();
            foreach (notification noti in _notifyContext.notify)
            {
                if (noti.BackInStock)
                {
                    prodList.Add(noti.productId.ToString() + " ");
                }
            }
           
            if(prodList != null)
            {
                ViewBag.flag = true;
                foreach (string s in prodList)
                {
                    proList.Add(_productContext.Products.Find(s));
                }
                products.all_products = proList;
            }
            else
            {
                ViewBag.messege = "no products arrived yet";
                
            }
            
            return PartialView("_popUpWindow", products);
        }

        public async Task<IActionResult> removeFromList(string Serial)
        {
            var user = await _userManager.GetUserAsync(User);
            notification? notify;
            string waitUs = "";
            notify = _notifyContext.notify.Find(Serial);

            string[] strings = notify.userId.Split();
            foreach (string s in strings)
            {
                if (!s.Equals(user.Id))
                {
                    waitUs = s;
                }
            }
            notify.userId = waitUs;
            
            if(waitUs.Equals(""))
            {
                _notifyContext.notify.Remove(notify);
            }
            else
            {
                _notifyContext.notify.Update(notify);
            }
            await _notifyContext.SaveChangesAsync();

            int countNoti = int.Parse(HttpContext.Request.Cookies["notification"]) - 1;
            if (countNoti == 0)
                HttpContext.Response.Cookies.Append("notification", "", new CookieOptions { Expires = DateTime.Now.AddDays(-1) });
            else
                HttpContext.Response.Cookies.Append("notification", countNoti.ToString());

            return View("Policy");
        }

        public async Task<IActionResult> notifyBtnPreesed(string Serial)
        {
            var user = await _userManager.GetUserAsync(User);
            notification? notify;
            string waitUs="";
            notify = _notifyContext.notify.Find(Serial);
            if(user == null)
            {
                return RedirectToAction("Index");
            }
            if (notify == null)
            {
                waitUs = user.Id +" ";
                notify = new notification
                {
                    productId = Serial,
                    userId = waitUs,
                    BackInStock = false
                };
                _notifyContext.notify.Add(notify);
            }
            else
            {
                string[] strings = notify.userId.Split();
                waitUs += user.Id + " ";
                foreach (string s in strings)
                {
                    if(!s.Equals(user.Id))
                    { 
                       waitUs += s + " ";
                    }
                    
                }
                notify.BackInStock = false;
                notify.productId = Serial.ToString();
                notify.userId = waitUs;
                _notifyContext.notify.Update(notify);
            }
            await _notifyContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }

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
