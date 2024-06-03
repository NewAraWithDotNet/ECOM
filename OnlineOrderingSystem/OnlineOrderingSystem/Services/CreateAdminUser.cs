using Microsoft.AspNetCore.Identity;
using OnlineOrderingSystem.Models;


public static class CreateAdmin
{
    public static async Task CreateAdminUser(IServiceProvider serviceProvider)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        // Check if the admin role exists
        var adminRoleExists = await roleManager.RoleExistsAsync("Admin");
        if (!adminRoleExists)
        {
            // Create the admin role if it doesn't exist
            await roleManager.CreateAsync(new IdentityRole("Admin"));
        }

        // Check if the admin user exists
        var adminUser = await userManager.FindByEmailAsync("admin@gmail.com");
        if (adminUser == null)
        {
            // Create the admin user if it doesn't exist
            adminUser = new User
            {
                UserName = "AdminUser",
                Email = "admin@gmail.com",
                Avatar = "https://i.ibb.co/nDKfswG/R.jpg"
            };
            await userManager.CreateAsync(adminUser, "Mm@123er53673");

            // Assign the admin role to the admin user
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }

}

