using Elaaj.Application.Features.Pharmacies.Dtos;
using Elaaj.Domain.Entities;
using Elaaj.Domain.Interfaces;
using Elaaj.infrastructure.Data;
using Elaaj.infrastructure.Repositories;
using Elaaj.infrastructure.Seeders;
using Elaaj.Infrastructure.Seeders;
using Elaaj.Application.Interfaces.Services; // Add this
using Elaaj.infrastructure.Services;         // Add this
using Elaaj.Application.Models;              // Add this
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Elaaj.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            services.AddIdentityApiEndpoints<User>()  
             .AddRoles<IdentityRole>() 
             .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddScoped<ISeeder, Seeder>();
            
            // Register Email Settings from appsettings.json
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));

            // Register IEmailService
            services.AddTransient<IEmailService, EmailService>();
         
            return services;
        }
    }
}