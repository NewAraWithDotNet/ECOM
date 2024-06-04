namespace OnlineOrderingSystem.ViewModels
{
    public class CartViewModel
    {
        public string UserName { get; set; }
        public List<CartItemViewModel> CartItems { get; set; }
    }

    public class CartItemViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; } 
        public decimal TotalPrice => Quantity * Price;

        public string? Image { get; internal set; }
    }
}
