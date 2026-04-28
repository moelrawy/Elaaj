using Elaaj.Domain.Entities;
using Elaaj.infrastructure.Data;
using Elaaj.infrastructure.Seeders;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Restaurants.Domain.Constants;

namespace Elaaj.Infrastructure.Seeders;

internal class Seeder(
    ApplicationDbContext dbContext,
    RoleManager<IdentityRole> roleManager)
    : ISeeder
{
    public async Task Seed()
    {
        if (dbContext.Database.GetPendingMigrations().Any())
        {
            await dbContext.Database.MigrateAsync();
        }

        if (await dbContext.Database.CanConnectAsync())
        {
            if (!await dbContext.Roles.AnyAsync())
            {
                await SeedRoles();
            }

            if (!await dbContext.Pharmacies.AnyAsync())
            {
                var pharmacies = GetPharmacies();
                dbContext.Pharmacies.AddRange(pharmacies);
                await dbContext.SaveChangesAsync();
            }
        }
    }

    private async Task SeedRoles()
    {
        string[] roles = [UserRoles.User, UserRoles.PharmacyAdmin, UserRoles.Owner];

        foreach (var roleName in roles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }
    }

    private IEnumerable<Pharmacy> GetPharmacies()
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
                HasDelivery = true
            },
            new() {
                Name = "Care Pharmacy",
                Address = "Assiut - University St.",
                ContactNumber = "01122334455",
                imageUrl = "https://cdn-icons-png.flaticon.com/512/883/883407.png",
                Latitude = 27.1850,
                Longitude = 31.1700,
                WorkingHours = "08:00 AM - 12:00 AM",
                HasDelivery = false
            }
        ];
    }
}