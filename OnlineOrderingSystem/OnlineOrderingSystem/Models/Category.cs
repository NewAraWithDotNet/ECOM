using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace OnlineOrderingSystem.Models
{
    public class Category
    {
        public int Id { get; set; }
        public required string Name { get; set; }

        public string? Image { get; set; }
        [NotMapped]
        [DisplayName("CategoryImage")]
        public IFormFile ImageFile { get; set; }
        public required ICollection<Product> Products { get; set; } = new List<Product>();

        public ICollection<Discount> Discounts { get; set; } = new List<Discount>();

    }
}
