using Elaaj.API.Extentions;
using Elaaj.API.Middlewares;
using Elaaj.Application.Extensions;
using Elaaj.Domain.Entities;
using Elaaj.infrastructure.Seeders;
using Elaaj.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


builder.AddPresentation();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

var app = builder.Build();

var scope = app.Services.CreateScope();
var seeder = scope.ServiceProvider.GetRequiredService<ISeeder>();

await seeder.Seed();

//app.UseMiddleware<ErrorHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
   
}

app.UseHttpsRedirection();

app.MapGroup("api/identity")
       .WithTags("Identity")
       .MapIdentityApi<User>();

app.UseAuthorization();

app.MapControllers();

app.Run();
