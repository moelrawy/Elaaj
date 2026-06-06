using Elaaj.Application.Features.Pharmacies.Dtos;
using Elaaj.Application.Interfaces;
using Elaaj.Application.Interfaces.Services; // Add this
using Elaaj.Application.Models;              // Add this
using Elaaj.Domain.Entities;
using Elaaj.Domain.Interfaces;
using Elaaj.infrastructure.Data;
using Elaaj.infrastructure.Repositories;
using Elaaj.infrastructure.Seeders;
using Elaaj.infrastructure.Services;         // Add this
using Elaaj.Infrastructure.Seeders;
using Elaaj.Infrastructure.Services;
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
    options.UseSqlServer(
        connectionString,
        sqlServerOptionsAction: sqlOptions =>
        {
            // 👇 السطر السحري اللي بيحل المشكلة
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5, // هيحاول 5 مرات قبل ما ييأس
                maxRetryDelay: TimeSpan.FromSeconds(30), // هيستنى ثواني بين كل محاولة
                errorNumbersToAdd: null);
        }));
            services.AddScoped<IApplicationDbContext, ApplicationDbContext>();

            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            services.AddIdentityApiEndpoints<User>()  
             .AddRoles<IdentityRole>() 
             .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddScoped<ISeeder, Seeder>();
            services.AddScoped<IChatNotificationService, ChatNotificationService>();
            // Register Email Settings from appsettings.json
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));

            // Register IEmailService
            services.AddTransient<IEmailService, EmailService>();
         
            return services;
        }
    }
}