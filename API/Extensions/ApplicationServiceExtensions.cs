using API.Services;
using Application.Features.Auth.Role;
using Application.Interfaces;
using Cortex.Mediator.DependencyInjection;
using InfraStructure.Repositories;
using InfraStructure.Services;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace API.Extensions;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(config.GetConnectionString("DefaultConnection")));
        services.AddCors(opt =>
        {
            opt.AddPolicy("CorsPolicy", policy =>
            {
                policy.AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()
                    .WithOrigins("http://localhost:3000");
            });
        });
        services.AddCortexMediator(
            configuration: config,
            handlerAssemblyMarkerTypes: new[] { typeof(RoleListHandler) }, 
            configure: options =>
            {
                options.AddDefaultBehaviors();
            }
        );
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IPermissionAuthorizationService, PermissionAuthorizationService>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IDatabaseSeeder, DatabaseSeeder>();
        services.AddScoped<IPermissionScanner, PermissionScanner>();
        services.AddScoped<UserService>();
        services.AddMemoryCache();
        return services;
    }
}