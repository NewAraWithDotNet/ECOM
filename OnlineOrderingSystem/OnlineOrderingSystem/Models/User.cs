using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineOrderingSystem.Models
{
    public class User : IdentityUser
    {
        
        public string Avatar { get; set; } = "https://i.ibb.co/Qrbkpcz/noavatar.jpg";
        [NotMapped]
        [DisplayName("UserImage")]
        public IFormFile AvatarFile { get; set; }

        public ICollection<ChatMessage> ChatMessages { get; set; }

    }
}
