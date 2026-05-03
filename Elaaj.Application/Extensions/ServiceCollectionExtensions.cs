using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Elaaj.Application.Users;

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
            return services;
        }
    }
}
