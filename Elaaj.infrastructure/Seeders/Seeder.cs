using Elaaj.Domain.Entities;
using Elaaj.infrastructure.Data;
using Elaaj.infrastructure.Seeders;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Restaurants.Domain.Constants;

namespace Elaaj.Infrastructure.Seeders;

internal class Seeder(
    ApplicationDbContext dbContext,
    RoleManager<IdentityRole> roleManager,
    UserManager<User> userManager)
    : ISeeder
{
    public async Task Seed()
    {
        if (await dbContext.Database.CanConnectAsync())
        {
            // ✅ نحاول ننفذ الـ Migrations لو في pending
            // لو الجداول موجودة بالفعل مش هيحصل حاجة
            var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();
            if (pendingMigrations.Any())
            {
                try
                {
                    await dbContext.Database.MigrateAsync();
                }
                catch (Exception)
                {
                    // الجداول موجودة بالفعل، مش مشكلة نكمل
                }
            }

            // 1. Seed Roles
            if (!await dbContext.Roles.AnyAsync())
            {
                await SeedRoles();
            }

            // 2. Seed Admin User (Owner)
            var adminEmail = "admin@elaaj.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new User
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "Admin System",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(adminUser, "Password123!");
                await userManager.AddToRoleAsync(adminUser, UserRoles.Owner);
            }

            // 3. Seed Pharmacies
            if (!await dbContext.Pharmacies.AnyAsync())
            {
                var pharmacies = GetPharmacies(adminUser.Id);
                dbContext.Pharmacies.AddRange(pharmacies);
                await dbContext.SaveChangesAsync();
            }
        }
    }

    private async Task SeedRoles()
    {
        string[] roles = [UserRoles.User, UserRoles.PharmacyAdmin, UserRoles.Owner, UserRoles.PharmacyOwner];

        foreach (var roleName in roles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }
    }

    private IEnumerable<Pharmacy> GetPharmacies(string ownerId)
    {
        return [
            new() {
                Name = "Al-Shifa Pharmacy",
                Address = "Assiut - Al-Nammis St.",
                ContactNumber = "01012345678",
                imageUrl = "https://cdn-icons-png.flaticon.com/512/4320/4320337.png",
                Latitude = 27.1809,
                Longitude = 31.1836,
                WorkingHours = "24/7",
                HasDelivery = true,
                OwnerId = ownerId
            },
            new() {
                Name = "Care Pharmacy",
                Address = "Assiut - University St.",
                ContactNumber = "01122334455",
                imageUrl = "https://cdn-icons-png.flaticon.com/512/883/883407.png",
                Latitude = 27.1850,
                Longitude = 31.1700,
                WorkingHours = "08:00 AM - 12:00 AM",
                HasDelivery = false,
                OwnerId = ownerId
            }
        ];
    }
}