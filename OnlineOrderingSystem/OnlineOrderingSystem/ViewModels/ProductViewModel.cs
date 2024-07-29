public class ProductViewModel
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int CategoryId { get; set; }
    public bool HasDiscount { get; set; }
    public int HowMany { get; set; }

    public decimal DiscountPrice { get; set; }
    public List<IFormFile> ImageFiles { get; set; } = new List<IFormFile>();
    public List<ProductOptionViewModel> ProductOptions { get; set; } = new List<ProductOptionViewModel>();
}

public class ProductOptionViewModel
{
    public string OptionName { get; set; }
    public decimal OptionPrice { get; set; }
}
    