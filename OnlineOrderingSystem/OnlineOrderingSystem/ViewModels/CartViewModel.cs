namespace OnlineOrderingSystem.ViewModels
{
    public class CartViewModel
    {
        public string Email { get; set; }
         public string Avatar { get; set; }
        public List<CartItemViewModel> CartItems { get; set; }
    }

    public class CartItemViewModel
    {
        public int CartItemId { get; set; }

        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; } 
        public decimal TotalPrice => Quantity * Price;

        public string? Image { get; internal set; }
        public List<ProductOptionViewModel> ProductOptions { get; set; }
    }
}
