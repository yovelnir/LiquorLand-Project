using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LiquorLand.Models;
using LiquorLand.ViewModels;
using Microsoft.AspNetCore.Identity;
using LiquorLand.Areas.Identity.Data;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Drawing;
using System.IO;

namespace LiquorLand.Controllers
{
    public class UserController : Controller
    {
        private readonly ProductContext _productContext;
        private readonly UserManager<Users> _userManager;

        public UserController(ProductContext productContext, UserManager<Users> userManager)
        {
            _productContext = productContext;
            _userManager = userManager;
        }

        [Route("Account/Manage")]
        [Route("User")]
        [Authorize]
        public IActionResult Index()
        {
            return View("Account/AccountManage");
        }

        [Authorize]
        public async Task<IActionResult> EditDetails(string? email, string? phone, string? fname, string? lname)
        {
            var user = await _userManager.GetUserAsync(User);
            bool valid = true;

            if (user != null)
            {
                if(email != user.Email && await _userManager.FindByEmailAsync(email) != null)
                {
                    ModelState.AddModelError(String.Empty, $"The email: {email} is already used by another user, please try again.");
                    valid = false;
                }
                else
                    user.Email = email;

                if (valid)
                {
                    user.PhoneNumber = phone;
                    user.FirstName = fname;
                    user.LastName = lname;

                    await _userManager.UpdateAsync(user);
                }
            }

            return View("Account/AccountManage");
        }

        public async Task<IActionResult> EditAddress(string? country, string? city, string? address, string? postal)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user != null)
            {
                user.Country = country;
                user.City = city;
                user.Address = address;
                user.PostalCode = postal;

                await _userManager.UpdateAsync(user);
            }

            return View("Account/AccountManage");
        }

        public async Task<IActionResult> ChangePassword(string currPass, string newPass)
        {
            var user = await _userManager.GetUserAsync(User);
            var result = await _userManager.ChangePasswordAsync(user, currPass, newPass);

            if (result.Succeeded)
            {
                return Json(new { success = true });
            }

            return Json(new { success = false });
        }

        #region !Admin Functions!

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
            if(ModelState.IsValid)
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

            return Redirect($"/item/{product.ProductName}");
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

    #endregion
}
