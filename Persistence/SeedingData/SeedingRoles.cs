using Microsoft.AspNetCore.Identity;
using YourExpo.Models;

namespace YourExpo.Persistence.SeedingData;

public class SeedingRoles
{
    public static async Task Initialize(IServiceProvider serviceProvider, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        string[] roleNames = { "Admin", "Supplier", "Customer" };
        foreach (var roleName in roleNames)
        {
            var roleExist = await roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        var adminUser = await userManager.FindByEmailAsync("admin@example.com");
        if (adminUser == null)
        {
            var user = new ApplicationUser
            {
                UserName = "admin@example.com",
                Email = "admin@example.com",
            };

            await userManager.CreateAsync(user, "Admin123!");

            // Assign Admin role to the user
            await userManager.AddToRoleAsync(user, "Admin");
        }
    }
}
