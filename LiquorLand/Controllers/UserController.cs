using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LiquorLand.Models;
using LiquorLand.ViewModels;
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
        public async Task<IActionResult> AddNewProduct(Product product, IFormFile? image)
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
                    else
                    {
                        product.ProductImage = Path.Combine("\\productImages", "No_Image_Available.jpg");
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

            ModelState.AddModelError(string.Empty, "Something went wrong");
            return RedirectProduct("AddProduct", product);
        }

        private IActionResult RedirectProduct(string view, Product product)
        {
            return View(view, product);
        }

        [Authorize(Roles = "Admin")]
        [Route("Admin/EditProduct")]
        public IActionResult Edit(Product product)
        {
            return View("EditProduct", product);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditProduct(Product product, IFormFile? image)
        {
            if (ModelState.IsValid)
            {
                Product? old_p = await _productContext.Products.FindAsync(product.Serial);
                if (old_p != null)
                {
                    if (image != null)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            product.RemoveImage(old_p.ProductImage);
                            await image.CopyToAsync(memoryStream);
                            product.ProductImage = await product.SaveImage(image);
                        }
                    }
                    else
                    {
                        product.ProductImage = old_p.ProductImage;
                    }
                    product.SaleAmount = old_p.SaleAmount;
                    _productContext.Entry<Product>(old_p).CurrentValues.SetValues(product);
                }
                await _productContext.SaveChangesAsync();
            }

            return RedirectToAction( "productsShow","Product", new { productName = product.ProductName });
        }

        //TO DO
        [Authorize(Roles = "Admin")]
        [Route("Admin/ProductManager")]
        public IActionResult ProductManager()
        {
            productViewModel products = new productViewModel();

            products.total_products = _productContext.Products.Count();
            products.all_products = _productContext.Products.Take(10).ToList<Product>();
            products.product_page = 1;

            return View("ProductManager", products);
        }
    }
}
