using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineOrderingSystem.Models
{
    public class Conversation
    {
        public int Id { get; set; }

        public string User1Id { get; set; }
        public User User1 { get; set; }

        public string User2Id { get; set; }
        public User User2 { get; set; }

        public virtual ICollection<ChatMessage> Messages { get; set; }
    }
}
