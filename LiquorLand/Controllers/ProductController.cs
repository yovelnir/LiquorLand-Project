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
    private readonly notifyContext _notifyContext;

    public ProductController(ProductContext productContext, notifyContext notifyContext)
    {
        _productContext = productContext;
        _notifyContext = notifyContext;
    }

    /*Product Page Loader*/
    public IActionResult productsShow(string ProductName)
    {
        Product? p;
        notification? notify;
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
                notify = _notifyContext.notify.Find(p.Serial);
                if(notify != null && p.Stock>0 && notify.BackInStock != true)
                {
                    notify.BackInStock=true;
                    _notifyContext.notify.Update(notify);
                    _notifyContext.SaveChangesAsync();

                    string? noti;
                    bool cookieExists = HttpContext.Request.Cookies.TryGetValue("notification", out noti);
                    
                    if(cookieExists)
                    {
                        int? countNoti = int.Parse(noti) + 1;
                    }
                    else
                        HttpContext.Response.Cookies.Append("notification", "1");
                }
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

    /*Filter Action after Searching for a product*/
    public IActionResult FilterProductSearch(Dictionary<string, string> filterData)
    {
        return PartialView("_GalleryFilter", unpackAndFilter(filterData));
    }

    /*Gallery Sorting Method*/
    public IActionResult SortGallery(Dictionary<string, string>? filterData)
    {
        productViewModel viewModel = unpackAndFilter(filterData);

        viewModel.all_products = viewModel.all_products.ToList<Product>();
        return PartialView("_GalleryFilter", viewModel);
    }

    /*Method to filter products for search term*/
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

    /*Search Products Method*/
    [Route("Search")]
    public IActionResult searchProduct(string word)
    {
        productViewModel products = searchFilter(word);
        if(products.all_products.Count == 1)
        {
            return RedirectToAction("productsShow", new {ProductName = products.all_products[0].ProductName});
        }

        products.total_products = products.all_products.Count();
        products.product_page = 1;
        products.all_products = products.all_products.Take(10).ToList<Product>();
        return View("ProductGallery",products);
    }

    [Route("ProductGallery")]
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

        products.total_products = products.all_products.Count();
        products.all_products = products.all_products.Take(10).ToList<Product>();
        products.product_page = 1;

        return View(products);
    }

    [HttpGet]
    public IActionResult loadMoreProducts(string? sort, Dictionary<string, string>? filterData)
    {
        productViewModel? viewModel = new productViewModel();

        if(sort != null)
            return SortGallery(filterData);
        else
            viewModel = unpackAndFilter(filterData);

        return PartialView("_GalleryFilter", viewModel);
    }

    /*Admin Product Management page*/
    [Authorize(Roles = "Admin")]
    public IActionResult FilterProductManager(Dictionary<string, string> filterData)
    {
        return PartialView("_ProductList", unpackAndFilter(filterData));
    }

    /*Remove Products From DataBase - Method can only be accessed by an admin!*/
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


    /*Product Filtering Method for all pages*/
    private productViewModel FilterProducts(string? serial, string? country, decimal? price, int? saleAmount, string? maker,
        string? category, string? sub, string? name, string? freeSearch, int? volume, decimal? alpercent, string? searchTerm, string? sort,
        int productAmount = 10, int page = 1, bool instock = false, bool outstock = false)
    {
        productViewModel? serachedProducts = null;
        if (searchTerm != null && searchTerm != "")
        {
            serachedProducts = searchFilter(searchTerm);
        }
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
        if (serial != null)
        {
            viewModel.all_products = products.Where<Product>(p => p.Serial == serial).ToList<Product>();

            return viewModel;
        }

        if (serachedProducts != null)
        {
            products = serachedProducts.all_products.AsQueryable();
        }

        //Query over country
        if (country != null)
            products = products.Where<Product>(p => p.Country.ToLower() == country.ToLower());
        //Query over price
        if (price != null)
            products = products.Where<Product>(p => p.ProductPrice >= price);
        //Query over sales amount
        if (saleAmount != null)
            products = products.Where<Product>(p => p.SaleAmount >= saleAmount);
        //Query over maker
        if (maker != null)
            products = products.Where<Product>(p => p.ProductMaker.ToLower().Contains(maker));
        //Query over category
        if (category != null)
            products = products.Where<Product>(p => p.ProductCategory.ToLower() == category.ToLower());
        //Query over sub category
        if (sub != null)
            products = products.Where<Product>(p => p.ProductSubCategory.ToLower() == sub.ToLower());
        //Query over name
        if (name != null)
            products = products.Where<Product>(p => p.ProductName.ToLower().Contains(name.ToLower()));
        //Query over description
        if (freeSearch != null)
            products = products.Where<Product>(p => p.ProductDescription.ToLower().Contains(freeSearch.ToLower()));
        //Query over volume
        if (volume != null)
            products = products.Where<Product>(p => p.Volume >= volume);
        //Query over alcohol percentage
        if (alpercent != null)
            products = products.Where<Product>(p => p.AlcoholPercentage >= alpercent);

        //Sort products by sort type
        if(sort != null)
        {
            switch (sort)
            {
                case "pup":
                    products = products.OrderByDescending(p => p.ProductPrice);
                    break;
                case "pdn":
                    products = products.OrderBy(p => p.ProductPrice);
                    break;
                case "nup":
                    products = products.OrderByDescending(p => p.ProductName);
                    break;
                case "ndn":
                    products = products.OrderBy(p => p.ProductName);
                    break;
                case "aup":
                    products = products.OrderByDescending(p => p.AlcoholPercentage);
                    break;
                case "adn":
                    products = products.OrderBy(p => p.AlcoholPercentage);
                    break;
                case "vup":
                    products = products.OrderByDescending(p => p.Volume);
                    break;
                case "vdn":
                    products = products.OrderBy(p => p.Volume);
                    break;
            }
        }

        //Saving total products in this filter session
        viewModel.total_products = products.Count();
        viewModel.product_page = page;

        //Final products list after SQL queries
        int take = productAmount * page;
        viewModel.all_products = products.Take(take).ToList<Product>();

        return viewModel;
    }

    /*Unpack the parameters for filtering from the filterData dictionary*/
    private productViewModel unpackAndFilter(Dictionary<string, string>? filterData)
    {
        string? serial = null, country = null, maker = null, category = null, sub = null, name = null, freeSearch = null, searchTerm = null, sort = null;
        decimal? price = null, alpercent = null;
        int? saleAmount = null, volume = null;
        bool instock = false, outstock = false;

        serial = filterData.ContainsKey("serial") ? filterData["serial"] : null;
        country = filterData.ContainsKey("country") ? filterData["country"] : null;
        maker = filterData.ContainsKey("maker") ? filterData["maker"] : null;
        category = filterData.ContainsKey("category") ? filterData["category"] : null;
        sub = filterData.ContainsKey("sub") ? filterData["sub"] : null;
        name = filterData.ContainsKey("name") ? filterData["name"] : null;
        searchTerm = filterData.ContainsKey("searchTerm") ? filterData["searchTerm"] : null;
        freeSearch = filterData.ContainsKey("freeSearch") ? filterData["freeSearch"] : null;
        sort = filterData.ContainsKey("sort") ? filterData["sort"] : null;
        decimal priceValue;
        price = filterData.ContainsKey("price") ? decimal.TryParse(filterData["price"], out priceValue) ? priceValue : null : null;
        decimal alch;
        alpercent = filterData.ContainsKey("alpercent") ? decimal.TryParse(filterData["alpercent"], out alch) ? alch : null : null;
        int vol;
        volume = filterData.ContainsKey("volume") ? int.TryParse(filterData["volume"], out vol) ? vol : null : null;
        int sales;
        saleAmount = filterData.ContainsKey("saleAmount") ? int.TryParse(filterData["saleAmount"], out sales) ? sales : null : null;
        instock = filterData.ContainsKey("instock") ? bool.Parse(filterData["instock"]) : false;
        outstock = filterData.ContainsKey("outstock") ? bool.Parse(filterData["outstock"]) : false;

        int productAmount = filterData.ContainsKey("productAmount") ? int.Parse(filterData["productAmount"]) : 10;
        int page = filterData.ContainsKey("page") ? int.Parse(filterData["page"]) : 1;

        return FilterProducts(serial, country, price, saleAmount, maker,
        category, sub, name, freeSearch, volume, alpercent, searchTerm, sort, productAmount, page, instock, outstock);

    }
}
