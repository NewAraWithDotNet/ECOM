using OnlineOrderingSystem.Models;

namespace OnlineOrderingSystem.ViewModels
{
    public class ProductSearchViewModel
    {
        public string Name { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? Category { get; set; }
        public List<Product> Results { get; set; }
    }
}
