using Microsoft.AspNetCore.Mvc;
using LiquorLand.Models;
using LiquorLand.Migrations;
using System.Data.SqlClient;
using Humanizer;
using LiquorLand.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;

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
}
