using AutoMapper;
using Elaaj.Application.Interfaces;
using Elaaj.Application.Interfaces.Services;
using Elaaj.Application.Users;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Elaaj.Application.Features.Users.Commands.RegisterUser;
using Elaaj.Application.Features.Identity;

namespace Elaaj.Application.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            var applicationAssembly = typeof(ServiceCollectionExtensions).Assembly;
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            //Mapping
            services.AddAutoMapper(cfg => {
                cfg.AddMaps(applicationAssembly);
            });
            services.AddHttpContextAccessor();
            services.AddScoped<IUserContext, UserContext>();
            services.AddScoped<IFileService, FileService>();
            services.AddTransient<IValidator<RegisterUserCommand>, RegisterUserCommandValidator>();
            return services;
        }
    }
}
