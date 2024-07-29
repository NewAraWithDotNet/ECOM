namespace OnlineOrderingSystem.Models
{
    public class ProductOption
    {
        public int Id { get; set; }
        public string OptionName { get; set; }
        public decimal OptionPrice { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}
