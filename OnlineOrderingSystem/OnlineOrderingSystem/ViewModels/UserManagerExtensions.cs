using Microsoft.AspNetCore.Identity;
using OnlineOrderingSystem.Models;
using System.Threading.Tasks;

namespace OnlineOrderingSystem.Extensions
{
    public static class UserManagerExtensions
    {
        public static async Task<bool> UpdateUsernameAsync(this UserManager<User> userManager, string currentUserName, string newUsername)
        {
            var user = await userManager.FindByNameAsync(currentUserName);
            if (user == null)
            {
                return false;
            }
            user.UserName = newUsername;
            var result = await userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public static async Task<bool> UpdateEmailAsync(this UserManager<User> userManager, string currentEmail, string newEmail)
        {
            var user = await userManager.FindByEmailAsync(currentEmail);
            if (user == null)
            {
                return false;
            }
            user.Email = newEmail;
            var result = await userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public static async Task<bool> UpdatePasswordAsync(this UserManager<User> userManager, string currentUserName, string currentPassword, string newPassword)
        {
            var user = await userManager.FindByNameAsync(currentUserName);
            if (user == null)
            {
                return false;
            }
            var result = await userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            return result.Succeeded;
        }

        public static async Task<bool> UpdateAvatarAsync(this UserManager<User> userManager, string currentUserName, string newAvatarPath)
        {
            var user = await userManager.FindByNameAsync(currentUserName);
            if (user == null)
            {
                return false;
            }
            user.Avatar = newAvatarPath;
            var result = await userManager.UpdateAsync(user);
            return result.Succeeded;
        }

    }
}
