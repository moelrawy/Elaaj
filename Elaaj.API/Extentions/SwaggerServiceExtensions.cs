using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace Elaaj.API.Extensions;

public static class SwaggerServiceExtensions
{
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            // 1. معلومات المشروع الأساسية
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "عِلاج API",
                Version = "v1",
                Description = "الواجهة البرمجية لتطبيق عِلاج للربط بين المرضى والصيدليات"
            });

            var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = System.IO.Path.Combine(System.AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);

            // 2. إعدادات الـ JWT عشان يظهر زرار الـ Authorize في Swagger
            var securitySchema = new OpenApiSecurityScheme
            {
                Description = "قم بإدخال التوكن هنا مباشرة (بدون كلمة Bearer)",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            };

            c.AddSecurityDefinition("Bearer", securitySchema);

            var securityRequirement = new OpenApiSecurityRequirement
            {
                { securitySchema, new[] { "Bearer" } }
            };

            c.AddSecurityRequirement(securityRequirement);
        });

        return services;
    }
}