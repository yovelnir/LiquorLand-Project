using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LiquorLand.Models;

namespace LiquorLand.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return Redirect("/");
        }

        [Authorize(Roles = "Admin")]
        [Route("AddProduct")]
        public IActionResult AddProduct()
        {
            return View();
        }

        public IActionResult AddNewProduct()
        {
            if (ModelState.IsValid)
            {
                return Redirect("/");
            }
            return Redirect("/policy");
        }
    }
}
