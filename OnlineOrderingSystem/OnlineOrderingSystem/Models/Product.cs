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
    }
}
