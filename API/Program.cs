using System.Text;
using API.Services;
using Application.Interfaces;
using InfraStructure.Repositories;
using InfraStructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["TokenKey"]));
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt=>{
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IPermissionAuthorizationService, PermissionAuthorizationService>();
builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();
builder.Services.AddScoped<IDatabaseSeeder, DatabaseSeeder>();
builder.Services.AddScoped<IPermissionScanner, PermissionScanner>();
builder.Services.AddScoped<TokenService>();
builder.Services.AddMemoryCache();

var app = builder.Build();

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
// Seed database
try
{
    //var context = services.GetRequiredService<AppDbContext>();
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
