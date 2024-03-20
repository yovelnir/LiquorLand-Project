using Microsoft.AspNetCore.Mvc;
using LiquorLand.Models;
using LiquorLand.Migrations;
using System.Data.SqlClient;
using Humanizer;
using LiquorLand.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;
using LiquorLand.ViewModels;

public class ProductController : Controller
{
    private readonly ProductContext _productContext;

    public ProductController(ProductContext productContext)
    {
        _productContext = productContext;
    }

    public IActionResult productsShow(string ProductName)
    {
        Product? p;
        if (ProductName == null)
        {
            string s = Request.Form["serial"];
            p = _productContext.Products.Find(s);
        }
        else
        {
            try
            {
                p = _productContext.Products.First(pr => pr.ProductName == ProductName);
            }
            catch
            {
                return View("404");
            }
        }

        return View("products", p);
    }


    public IActionResult Show(Product p)
    {
        return View("products", p);
    }
    public IActionResult searchProduct(string word ="wine")
    {
        productViewModel products = new productViewModel();
        foreach(Product p in _productContext.Products)
        {
            products.all_products.Add(p);
        }
        ViewData["searchWord"] = word;
        return View("Search", products);
    }

    public async Task<IActionResult> ProductGallery(string? category, string? sub)
    {
        productViewModel products = new productViewModel();
        if (category != null && sub != null)
        {
            products.all_products = await _productContext.Products
                .Where<Product>(p => p.ProductCategory == category && p.ProductSubCategory == sub) 
                .ToListAsync();
        }
        else if (category != null)
        {
            products.all_products = await _productContext.Products
                .Where<Product>(p => p.ProductCategory == category)
                .ToListAsync();
        }
        else
        {
            products.all_products = await _productContext.Products.ToListAsync();
        }

        if(products.all_products.Count == 0)
        {
            ModelState.AddModelError(string.Empty, "No products found");
        }

        return View(products);
    }
}
