using Microsoft.AspNetCore.Mvc;
using LiquorLand.Models;
using LiquorLand.Migrations;
using System.Data.SqlClient;
using Humanizer;
using LiquorLand.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;
using LiquorLand.ViewModels;
using Microsoft.AspNetCore.Authorization;

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

    [Authorize(Roles = "Admin")]
    public IActionResult FilterProductManager(string? serial, string? country, decimal? price, int? saleAmount, string? maker,
        string? category, string? sub, string? name, string? freeSearch, int? volume, decimal? alpercent, bool instock = false, bool outstock = false)
    {
        //View Model to hold the products list
        productViewModel viewModel = new productViewModel();

        //Queryable variable to handle all queries
        IQueryable<Product> products = _productContext.Products;

        //Query over in stock products
        if (instock)
            products = products.Where<Product>(p => p.Stock > 0);
        //Query over out of stock products
        else if (outstock)
            products = products.Where<Product>(p => p.Stock == 0);

        //Query over serial, if you query over serial the model is returned to the partial view with only one product in the list!
        if(serial != null)
        {
            viewModel.all_products = products.Where<Product>(p => p.Serial == serial).ToList<Product>();

            return PartialView("_ProductList", viewModel);
        }

        //Query over country
        if (country != null)
            products = products.Where<Product>(p => p.Country == country);
        //Query over price
        if (price != null)
            products = products.Where<Product>(p => p.ProductPrice >= price);
        //Query over sales amount
        if (saleAmount != null)
            products = products.Where<Product>(p => p.SaleAmount >= saleAmount);
        //Query over maker
        if (maker != null)
            products = products.Where<Product>(p => p.ProductMaker.Contains(maker));
        //Query over category
        if (category != null)
            products = products.Where<Product>(p => p.ProductCategory == category);
        //Query over sub category
        if (sub != null)
            products = products.Where<Product>(p => p.ProductSubCategory == sub);
        //Query over name
        if (name != null)
            products = products.Where<Product>(p => p.ProductName.Contains(name));
        //Query over description
        if (freeSearch != null)
            products = products.Where<Product>(p => p.ProductDescription.Contains(freeSearch));
        //Query over volume
        if (volume != null)
            products = products.Where<Product>(p => p.Volume >= volume);
        //Query over alcohol percentage
        if (alpercent != null)
            products = products.Where<Product>(p => p.AlcoholPercentage >= alpercent);

        //Final products list after SQL queries
        viewModel.all_products = products.ToList<Product>();

        return PartialView("_ProductList", viewModel);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RemoveProduct(string serial)
    {
        Product? product = await _productContext.Products.FindAsync(serial);
        //Remove the product from the database
        if (product != null)
        {
            product.RemoveImage(product.ProductImage);
            _productContext.Products.Remove(product);
        }

        //Handeling multiple requests to save database changes
        await _productContext.SaveChangesAsync();

        return RedirectToAction("ProductManager", "User");
    }
}
