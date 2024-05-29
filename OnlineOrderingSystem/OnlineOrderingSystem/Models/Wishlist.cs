namespace OnlineOrderingSystem.Models
{
    public class Wishlist
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public required User User { get; set; }
        public required ICollection<Product> Products { get; set; }
    }
}
