using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LiquorLand.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Drawing;
using System.IO;

namespace LiquorLand.Controllers
{
    public class UserController : Controller
    {
        private readonly ProductContext _productContext;

        public UserController(ProductContext productContext)
        {
            _productContext = productContext;
        }

        public IActionResult Index()
        {
            return Redirect("/");
        }

        [Authorize(Roles = "Admin")]
        [Route("Admin/AddProduct")]
        public IActionResult AddProduct()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        [Route("AddProduct")]
        public async Task<IActionResult> AddNewProduct(Product product, IFormFile image)
        {
            if (ModelState.IsValid)
            {
                var result = await _productContext.Products.FindAsync(product.Serial);

                if (result == null)
                {
                    if (image != null)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await image.CopyToAsync(memoryStream);
                            product.ProductImage = await product.SaveImage(image);
                        }

                    }
                    await _productContext.AddAsync(product);
                    await _productContext.SaveChangesAsync();
                    
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Product Serial already exists");
                    return RedirectProduct("AddProduct", product);
                }

                return Redirect("/");
            }

            return Redirect("/policy");
        }

        private IActionResult RedirectProduct(string view, Product product)
        {
            return View(view, product);
        }
    }
}
