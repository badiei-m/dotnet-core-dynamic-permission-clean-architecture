using API.Services;
using Application.Interfaces;
using InfraStructure.Repositories;
using InfraStructure.Services;
using Microsoft.EntityFrameworkCore;
using Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IPermissionAuthorizationService, PermissionAuthorizationService>();
builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();
builder.Services.AddScoped<IDatabaseSeeder, DatabaseSeeder>();
builder.Services.AddScoped<IPermissionScanner, PermissionScanner>();
builder.Services.AddMemoryCache();

//builder.Services.AddScoped<IProductService, ProductService>();
var app = builder.Build();

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
// Seed database
try
{
    var context = services.GetRequiredService<AppDbContext>();
    //await context.Database.MigrateAsync();
    var seeder = services.GetRequiredService<IDatabaseSeeder>();
    await seeder.SeedAsync();
}
catch (Exception Ex)
{
    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogError(Ex,"Err");
    throw;
}
// using (var scope = app.Services.CreateScope())
// {
//     var context = services.GetRequiredService<DataContext>();
//     var userManager = services.GetRequiredService<UserManager<AppUser>>();
//     await context.Database.MigrateAsync();
//     await Seed.SeedData(context,userManager);
//     
//
// }

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}





app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
