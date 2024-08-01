using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OnlineOrderingSystem.Models
{
    public class Discount
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public bool IsActive { get; set; } = true;
        public decimal DiscountPercentage { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
