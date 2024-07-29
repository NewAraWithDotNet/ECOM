using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace OnlineOrderingSystem.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string? Image { get; set; }
        [NotMapped]
        [DisplayName("CategoryImage")]
        public IFormFile ImageFile { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public ICollection<Comment> Comments { get; set; }
        
        public int HowMany { get; set; }

        public bool CommentsEnabled { get; set; } = true;
        public ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();

        public bool HasDiscount { get; set; }
        public decimal DiscountPrice { get; set; }

        public ICollection<ProductOption> ProductOptions { get; set; } = new List<ProductOption>();

    }
}
