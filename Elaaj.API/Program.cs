using Elaaj.API.Extentions;
using Elaaj.API.Hubs;
using Elaaj.API.Middlewares;
using Elaaj.API.Services;
using Elaaj.Application.Extensions;
<<<<<<< Updated upstream
=======
using Elaaj.Application.Interfaces;
using Elaaj.Domain.Entities;
using Elaaj.infrastructure.Data;
>>>>>>> Stashed changes
using Elaaj.infrastructure.Seeders;
using Elaaj.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
//Add SignalR services
builder.Services.AddSignalR();
// Add services to the container.


builder.AddPresentation();

// MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();
// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials()
              .SetIsOriginAllowed(host => true);
    });
});



var app = builder.Build();
var scope = app.Services.CreateScope();
var seeder = scope.ServiceProvider.GetRequiredService<ISeeder>();
//CORS
app.UseCors("AllowAll");


await seeder.Seed();

app.UseMiddleware<ErrorHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
   
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

app.MapHub<NotificationHub>("/notificationsHub");

app.Run();
