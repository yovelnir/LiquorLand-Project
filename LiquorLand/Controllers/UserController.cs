using Microsoft.AspNetCore.Mvc;

namespace LiquorLand.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
