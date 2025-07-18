using System.Reflection;
using Domain.Entities.System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Persistence;

public class DbSeeder
{
    private readonly IServiceProvider _provider;

    public DbSeeder(IServiceProvider provider)
    {
        _provider = provider;
    }

    public async Task SeedAsync(Assembly webApiAssembly)
    {
        using var scope = _provider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // 1. Create Admin Role
        var adminRole = await context.Entity<Role>().FirstOrDefaultAsync(r => r.Name == "Admin");
        if (adminRole == null)
        {
            adminRole = new Role { Name = "Admin" };
            context.Entity<Role>().Add(adminRole);
            await context.SaveChangesAsync();
        }

        // 2. Create Admin User
        var adminUser = await context.Entity<User>().FirstOrDefaultAsync(u => u.Username == "admin");
        if (adminUser == null)
        {
            adminUser = new User
            {
                Username = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("yourPassword123") // Use BCrypt or ASP.NET Identity style
            };
            context.Entity<User>().Add(adminUser);
            await context.SaveChangesAsync();
        }

        // 3. Assign Admin Role to Admin User
        if (!await context.Entity<UserRole>().AnyAsync(ur => ur.UserId == adminUser.Id && ur.RoleId == adminRole.Id))
        {
            context.Entity<UserRole>().Add(new UserRole
            {
                UserId = adminUser.Id,
                RoleId = adminRole.Id
            });
            await context.SaveChangesAsync();
        }

        // 4. Scan Controller Actions
        // var permissions = new List<Permission>();
        //
        // var controllerTypes = webApiAssembly.GetTypes()
        //     .Where(t => t.IsSubclassOf(typeof(ControllerBase)));
        //
        // foreach (var controller in controllerTypes)
        // {
        //     var controllerName = controller.Name.Replace("Controller", "");
        //
        //     var methods = controller.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
        //         .Where(m => !m.IsDefined(typeof(AllowAnonymousAttribute), true));
        //
        //     foreach (var method in methods)
        //     {
        //         var actionName = method.Name;
        //
        //         var permissionName = $"{controllerName}.{actionName}";
        //
        //         if (!context.Entity<Permission>().Any(p => p.Key == permissionName))
        //         {
        //             permissions.Add(new Permission
        //             {
        //                 Key = permissionName,
        //                 Description = $"Auto-added permission for {permissionName}"
        //             });
        //         }
        //     }
        // }
        //
        // if (permissions.Any())
        // {
        //     context.Permissions.AddRange(permissions);
        //     context.SaveChanges();
        // }
        
        // var permissionsFromCode = GetPermissionsFromCode();
        // foreach (var key in permissionsFromCode)
        // {
        //     var permission = await context.Entity<Permission>().FirstOrDefaultAsync(p => p.Key == key);
        //     if (permission == null)
        //     {
        //         permission = new Permission { Key = key, Description = key };
        //         context.Entity<Permission>().Add(permission);
        //         await context.SaveChangesAsync();
        //     }
        //
        //     var rolePermExists = await context.Entity<RolePermission>()
        //         .AnyAsync(rp => rp.RoleId == adminRole.Id && rp.PermissionId == permission.Id);
        //
        //     if (!rolePermExists)
        //     {
        //         context.Entity<RolePermission>().Add(new RolePermission
        //         {
        //             RoleId = adminRole.Id,
        //             PermissionId = permission.Id
        //         });
        //         await context.SaveChangesAsync();
        //     }
        // }
    }

    // private List<string> GetPermissionsFromCode()
    // {
    //     // var permissions = new List<string>();
    //     // var controllers = Assembly.GetExecutingAssembly()
    //     //     .GetTypes()
    //     //     .Where(t => typeof(ControllerBase).IsAssignableFrom(t));
    //
    //     // foreach (var controller in controllers)
    //     // {
    //     //     var ctrlName = controller.Name.Replace("Controller", "");
    //     //     var methods = controller.GetMethods()
    //     //         .Where(m => m.GetCustomAttributes(typeof(MustHavePermissionAttribute), true).Any());
    //     //
    //     //     foreach (var method in methods)
    //     //     {
    //     //         permissions.Add($"{ctrlName}.{method.Name}");
    //     //     }
    //     // }
    //
    //     return permissions.Distinct().ToList();
    // }
}