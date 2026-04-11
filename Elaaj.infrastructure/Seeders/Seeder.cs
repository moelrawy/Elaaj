
using Elaaj.Domain.Entities;
using Elaaj.infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Elaaj.infrastructure.Seeders;

public class Seeder(ApplicationDbContext dbContext) : ISeeder
{
    public async Task Seed()
    {
        if(dbContext.Database.GetPendingMigrations().Any())
        {
            await dbContext.Database.MigrateAsync();
        }

        if(await dbContext.Database.CanConnectAsync())
        {
            // Seed Pharmacies
            if (!dbContext.Pharmacies.Any())
            {
                var pharmacies = GetPharmacies();
                dbContext.Pharmacies.AddRange(pharmacies);
                await dbContext.SaveChangesAsync();
            }

            // Seed Patients
            if (!dbContext.Patients.Any())
            {
                var patients = GetPatients();
                dbContext.Patients.AddRange(patients);
                await dbContext.SaveChangesAsync();
            }

            // Seed Posts & Replies 
            if (!dbContext.Posts.Any())
            {
                var patient = await dbContext.Patients.FirstOrDefaultAsync();
                var pharmacy = await dbContext.Pharmacies.FirstOrDefaultAsync();

                if (patient != null && pharmacy != null)
                {
                    var posts = GetPosts(patient.Id, pharmacy.Id);
                    dbContext.Posts.AddRange(posts);
                    await dbContext.SaveChangesAsync();
                }
            }

        }
    }

    private IEnumerable<Pharmacy> GetPharmacies()
    {
        List<Pharmacy> pharmacies = [
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

        return pharmacies;
    }

    private IEnumerable<Patient> GetPatients()
    {
        List<Patient> patients = 
        [
         new() { FullName = "Mohammed Hassan", Region = "Asyut - City Center" },
         new() { FullName = "Ahmed Morsi", Region = "Asyut - University District" }
        ];

        return patients;
    }

    private IEnumerable<Post> GetPosts(int patientId, int pharmacyId)
    {
        List<Post> posts = [
        new() {
            Content = "Is there an alternative for Panadol Cold & Flu?",
            CreatedAt = DateTime.UtcNow,
            PatientId = patientId,
            ImageUrl = "https://example.com/med-post.png",
            postReplies = [
                new() {
                    Message = "Yes, you can use Adol Sinus.",
                    CreatedAt = DateTime.UtcNow,
                    PharmacyId = pharmacyId
                }
            ]
        }
    ];

        return posts;
    }
}
