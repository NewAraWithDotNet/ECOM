
namespace OnlineOrderingSystem.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public User user { get; set; }
        public ICollection<CartItem> CartItems { get; set; }

       
    }
}
