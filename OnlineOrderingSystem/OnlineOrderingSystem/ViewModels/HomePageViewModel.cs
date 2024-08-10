using OnlineOrderingSystem.Models;

public class HomePageViewModel
{
    public IEnumerable<Category> Categories { get; set; }
    public IEnumerable<Product> TopProducts { get; set; }
}
