using Microsoft.AspNetCore.Identity;

namespace CRMSystem.Data
{
    public static class DbInitializer
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
        {
            // Get RoleManager and UserManager from DI container
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            // Define roles needed in the system
            string[] roles = { "Admin", "Manager", "SalesAssociate", "MarketingAssociate", "SupportAgent", "Report" };

            // Create roles if they don't exist
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    var result = await roleManager.CreateAsync(new IdentityRole(role));
                    if (result.Succeeded)
                    {
                        Console.WriteLine($"Role created: {role}");
                    }
                    else
                    {
                        Console.WriteLine($"Failed to create role: {role}");
                    }
                }
            }

            // Define default users for each role
            var defaultUsers = new[]
            {
                new { Email = "admin@crm.com", Password = "Admin@123", Role = "Admin" },
                new { Email = "manager@crm.com", Password = "Manager@123", Role = "Manager" },
                new { Email = "sales@crm.com", Password = "Sales@123", Role = "SalesAssociate" },
                new { Email = "marketing@crm.com", Password = "Marketing@123", Role = "MarketingAssociate" },
                new { Email = "support@crm.com", Password = "Support@123", Role = "SupportAgent" },
                new { Email = "report@crm.com", Password = "Report@123", Role = "Report" }
            };

            // Create users and assign roles
            foreach (var userInfo in defaultUsers)
            {
                var user = await userManager.FindByEmailAsync(userInfo.Email);
                if (user == null)
                {
                    var newUser = new IdentityUser
                    {
                        UserName = userInfo.Email,
                        Email = userInfo.Email,
                        EmailConfirmed = true
                    };

                    var result = await userManager.CreateAsync(newUser, userInfo.Password);
                    if (result.Succeeded)
                    {
                        // Ensure role exists before assigning
                        if (await roleManager.RoleExistsAsync(userInfo.Role))
                        {
                            await userManager.AddToRoleAsync(newUser, userInfo.Role);
                        }
                    }
                }
            }
        }
    }
}