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

    public IActionResult productsShow(Product p)
    {
        return View("products", p);
    }
}
