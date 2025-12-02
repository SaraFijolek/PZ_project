using Microsoft.AspNetCore.Identity;
using Pharmacy_API.Models;

namespace Pharmacy_API.Data
{
    public class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            if (serviceProvider == null) throw new ArgumentNullException(nameof(serviceProvider));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            var roleManager = serviceProvider.GetService<RoleManager<IdentityRole<int>>>();
            if (roleManager == null) throw new Exception("RoleManager nie został zarejestrowany w DI");

            var userManager = serviceProvider.GetService<UserManager<ApplicationUser>>();
            if (userManager == null) throw new Exception("UserManager nie został zarejestrowany w DI");

           
            var roles = new[] { "Admin", "Worker" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    var roleResult = await roleManager.CreateAsync(new IdentityRole<int>(role));
                    if (!roleResult.Succeeded)
                        throw new Exception($"Błąd tworzenia roli {role}: {string.Join(", ", roleResult.Errors)}");
                }
            }

            
            var adminUserName = configuration["SeedAdmin:UserName"] ?? "admin@pharmacy.local";
            var adminEmail = configuration["SeedAdmin:Email"] ?? adminUserName;
            var adminPassword = configuration["SeedAdmin:Password"] ?? "ChangeMe123!";

            var existing = await userManager.FindByEmailAsync(adminEmail);
            if (existing == null)
            {
                var admin = new ApplicationUser
                {
                    UserName = adminUserName,
                    Email = adminEmail,
                    Imie = "Admin",
                    Nazwisko = "Boss",
                    Data_zatrudnienia = DateTime.UtcNow,
                    Zmiana = "Dzienna",
                    Admin = true
                };

                var createResult = await userManager.CreateAsync(admin, adminPassword);
                if (!createResult.Succeeded)
                    throw new Exception("Nie udało się utworzyć seed admina: " + string.Join(", ", createResult.Errors));

                var addRoleResult = await userManager.AddToRoleAsync(admin, "Admin");
                if (!addRoleResult.Succeeded)
                    throw new Exception("Nie udało się przypisać roli Admin seed owemu userowi: " + string.Join(", ", addRoleResult.Errors));
            }
        }
    }
}
