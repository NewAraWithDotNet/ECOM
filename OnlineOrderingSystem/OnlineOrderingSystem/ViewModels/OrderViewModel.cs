using OnlineOrderingSystem.Models;

namespace OnlineOrderingSystem.ViewModels
{
    public class OrderViewModel
    {
        public Order Order { get; set; }
        public List<OrderItem> OrderItems { get; set; }
    }
}