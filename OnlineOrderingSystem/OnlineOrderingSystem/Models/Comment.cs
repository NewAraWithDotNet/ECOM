using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineOrderingSystem.Models
{
    public class Comment
    {
        public int Id { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public DateTime DatePosted { get; set; }

        [Required]
        public string UserId { get; set; }

        public User user { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public Product Product { get; set; }

        [Range(1, 5)]
        public int? Rating { get; set; }
    }
}
