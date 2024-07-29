namespace OnlineOrderingSystem.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public int CartId { get; set; }
        public int ProductId { get; set; }
        public int? ProductOptionId { get; set; } 
        public decimal Price { get; set; }
        public int Quantity { get; set; }

        public Cart Cart { get; set; }
        public Product Product { get; set; }
        public ProductOption ProductOption { get; set; }
    }


}
