using Microsoft.AspNetCore.Identity;

namespace ticketApp.Models
{
    public class AppSeedData
    {
        public static async Task SeedAsync(IApplicationBuilder app)
        {
            var userManager = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<UserManager<AppUser>>();

            var roleManager = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<RoleManager<AppRole>>();

            string[] roles = { "Admin", "Developer", "User" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new AppRole { Name = role });
            }

            var adminEmail = "admin@ticketapp.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                var user = new AppUser { UserName = "admin", Email = adminEmail };
                await userManager.CreateAsync(user, "Admin123*");
                await userManager.AddToRoleAsync(user, "Admin");
            }
        }
    }
}