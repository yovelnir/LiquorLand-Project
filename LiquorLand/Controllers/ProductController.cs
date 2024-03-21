using Microsoft.AspNetCore.Mvc;
using LiquorLand.Models;
using LiquorLand.Migrations;
using System.Data.SqlClient;
using Humanizer;
using LiquorLand.Areas.Identity.Data;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using LiquorLand.ViewModels;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json.Serialization;


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

    public IActionResult FilterProductSearch(string? serial, string? country, decimal? price, int? saleAmount, string? maker,
       string? category, string? sub, string? name, int? volume, decimal? alpercent, string? searchTerm)
    {
        productViewModel? product = null;
        if (searchTerm != null && searchTerm != "")
        {
            product = searchFilter(searchTerm);
        }
        //View Model to hold the products list
        productViewModel viewModel = new productViewModel();
        
        //Queryable variable to handle all queries
        IQueryable<Product> products = _productContext.Products;

        //Query over serial, if you query over serial the model is returned to the partial view with only one product in the list!
        if (serial != null)
        {
            viewModel.all_products = products.Where<Product>(p => p.Serial == serial).ToList<Product>();

            return PartialView("Search", viewModel);
        }

        if (product != null)
        {
            products = product.all_products.AsQueryable();
        }

        //Query over country
        if (country != null && products.Any(p => p.Country == country))
            products = products.Where<Product>(p => p.Country == country);
        //Query over price
        if (price != null && products.Any(p => p.ProductPrice >= price))
            products = products.Where<Product>(p => p.ProductPrice >= price);
        //Query over sales amount
        if (saleAmount != null && products.Any(p => p.SaleAmount >= saleAmount))
            products = products.Where<Product>(p => p.SaleAmount >= saleAmount);
        //Query over maker
        if (maker != null && products.Any(p => p.ProductMaker.ToLower().Contains(maker)))
            products = products.Where<Product>(p => p.ProductMaker.ToLower().Contains(maker));
        //Query over category
        if (category != null && products.Any(p => p.ProductCategory == category))
            products = products.Where<Product>(p => p.ProductCategory == category);
        //Query over sub category
        if (sub != null)
            products = products.Where<Product>(p => p.ProductSubCategory == sub);
        //Query over name
        if (name != null)
            products = products.Where<Product>(p => p.ProductName.Contains(name));
        //Query over volume
        if (volume != null)
            products = products.Where<Product>(p => p.Volume >= volume);
        //Query over alcohol percentage
        if (alpercent != null && products.Any(p => p.AlcoholPercentage >= alpercent))
            products = products.Where<Product>(p => p.AlcoholPercentage >= alpercent);

        //Final products list after SQL queries
        viewModel.all_products = products.ToList<Product>();


        return PartialView("Search", viewModel);
    }

    private productViewModel searchFilter(string word)
    {
        productViewModel products = new productViewModel();
        if (word != null)
            foreach (Product p in _productContext.Products)
            {
                if (p.ProductCategory.ToLower().Contains(word.ToString().ToLower()) ||
                    p.ProductName.ToLower().Contains(word.ToString().ToLower()) ||
                    p.Serial.ToLower().Contains(word.ToString().ToLower()) ||
                    p.ProductSubCategory.ToLower().Contains(word.ToString().ToLower()) ||
                    p.Country.ToLower().Contains(word.ToString().ToLower()))
                {
                    products.all_products.Add(p);
                }
            }

        return products;
    }
    public IActionResult searchProduct(string word)
    {
        productViewModel products = searchFilter(word);
        //ViewData["searchWord"] = word;
        //return View("Search", products);
        //return RedirectToAction("ProductGallery","Product",products);
        return View("ProductGallery",products);
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
