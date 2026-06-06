using Elaaj.API.Extensions;
using Elaaj.API.Extentions;
using Elaaj.API.Middlewares;
using Elaaj.infrastructure.Services;
using Elaaj.Application.Extensions;
using Elaaj.Application.Interfaces;
using Elaaj.Domain.Entities;
using Elaaj.infrastructure.Data;
using Elaaj.Infrastructure.Hubs;
using Elaaj.infrastructure.Seeders;
using Elaaj.Infrastructure.Extensions;
using Elaaj.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddIdentityServices(builder.Configuration);
builder.AddPresentation();
//configurationservices
builder.Services.AddSignalR(options =>
{ 
    options.EnableDetailedErrors = true;
});

/*builder.Services.AddSignalR().AddStackExchangeRedis("your_redis_connection_string", options => {
    options.Configuration.ChannelPrefix = "Elaaj_Notification_Hub";
});
*/
// MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

builder.Services.AddScoped<INotificationService,NotificationService>();
builder.Services.AddSwaggerDocumentation();
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
app.UseStaticFiles();
var scope = app.Services.CreateScope();
var seeder = scope.ServiceProvider.GetRequiredService<ISeeder>();
//CORS
app.UseCors("AllowAll");


await seeder.Seed();

//app.UseMiddleware<ErrorHandlingMiddleware>();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Elaaj API v1");
});

app.UseHttpsRedirection();

app.MapGroup("api/identity")
       .WithTags("Identity")
       .MapIdentityApi<User>();

app.UseAuthorization();

app.MapControllers();


// OR inside your AddApplication() or AddInfrastructure() extension methods
app.MapHub<NotificationHub>("/NotificationHub");

app.Run();
