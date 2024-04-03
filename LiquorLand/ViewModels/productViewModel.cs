using LiquorLand.Models;


namespace LiquorLand.ViewModels
{
    public class productViewModel
    {
        public List<Product> all_products = new List<Product>();
        public Product? product { get; set; }
        public List<Product> hot_products = new List<Product>();
        public int? total_products;
        public int? product_page;
     
        
        public List<Product>? getHotProducts(ProductContext _productContext)
        {
            Random rand = new Random();
            int skip = rand.Next(0, 3);
            List<Product>? hotProducts = _productContext.Products.OrderByDescending(p => p.SaleAmount)
                .Skip(skip).Take(6).ToList<Product>();

            return hotProducts;
        }
    }
}
