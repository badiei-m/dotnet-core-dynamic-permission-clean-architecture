using Application.Interfaces;
using Domain.Entities.System;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace InfraStructure.Services;

public class DatabaseSeeder(
    AppDbContext context,
    IPermissionScanner permissionScanner)
    : IDatabaseSeeder
{
    public async Task SeedAsync()
    {
        // Ensure database is created
        await context.Database.EnsureCreatedAsync();

        // Seed permissions from controllers
        await SeedPermissionsFromControllersAsync();

        // Seed admin role
        await SeedAdminRoleAsync();

        // Seed admin user
        await SeedAdminUserAsync();
        
        // Seed Common role
        await SeedCommonRoleAsync();
        
        //Seed Common User
        await SeedCommonUserAsync();

        // Assign all permissions to admin role
        await AssignPermissionsToAdminRoleAsync();
    }

    private async Task SeedPermissionsFromControllersAsync()
    {
        var permissionNames = permissionScanner.GetPermissionNames();
        var permissionsToAdd = new List<Permission>();

        foreach (var permissionName in permissionNames)
        {
            if (!context.Entity<Permission>().Any(p => p.Key == permissionName))
            {
                permissionsToAdd.Add(new Permission
                {
                    Key = permissionName,
                    Description =
                        $"Permission to {permissionName.Split('.').Last()} {permissionName.Split('.').First()}"
                });
            }
        }

        if (permissionsToAdd.Any())
        {
            context.Entity<Permission>().AddRange(permissionsToAdd);
            await context.SaveChangesAsync();
        }
    }

    private async Task SeedAdminRoleAsync()
    {
        const string adminRoleName = "Admin";
        if (!context.Entity<Role>().Any(r => r.Name == adminRoleName))
        {
            var adminRole = new Role { Name = adminRoleName };
            context.Entity<Role>().Add(adminRole);
            await context.SaveChangesAsync();
        }
    }

    private async Task SeedCommonRoleAsync()
    {
        const string adminRoleName = "Common";
        if (!context.Entity<Role>().Any(r => r.Name == adminRoleName))
        {
            var adminRole = new Role { Name = adminRoleName };
            context.Entity<Role>().Add(adminRole);
            await context.SaveChangesAsync();
        }
    }

    private async Task SeedCommonUserAsync()
    {
        const string username = "common";
        const string password = "yourPassword";
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
        
        var permissionTesterUser = await context.Entity<User>()
            .FirstOrDefaultAsync(x=>x.UserName == username);

        if (permissionTesterUser == null)
        {
            permissionTesterUser = new User
            {
                UserName = username,
                PasswordHash = hashedPassword,
                Email = "common@test.com",
                DisplayName = "Common User"
            };
            context.Entity<User>().Add(permissionTesterUser);
            await context.SaveChangesAsync();
        }
    }
    private async Task SeedAdminUserAsync()
    {
        const string adminUsername = "admin";
        const string adminPassword = "yourPassword";
        
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(adminPassword);
        
        var adminUser = await context.Entity<User>()
            .FirstOrDefaultAsync(x=>x.UserName == adminUsername);
        if (adminUser == null)
        {
            adminUser = new User
            {
                UserName = adminUsername,
                PasswordHash = hashedPassword,
                Email = "admin@test.com",
                DisplayName = "Admin User"
            };
            context.Entity<User>().Add(adminUser);
            await context.SaveChangesAsync();

            // Add to User table
            if (!context.Entity<User>().Any(u => u.Id == adminUser.Id))
            {
                context.Entity<User>().Add(new User { Id = adminUser.Id, UserName = adminUsername });
                await context.SaveChangesAsync();
            }

            // Assign to Admin role
            var adminRole = context.Entity<Role>().FirstOrDefault(r => r.Name == "Admin");
            if (adminRole != null &&
                !context.Entity<UserRole>().Any(ur => ur.UserId == adminUser.Id && ur.RoleId == adminRole.Id))
            {
                context.Entity<UserRole>().Add(new UserRole { UserId = adminUser.Id, RoleId = adminRole.Id });
                await context.SaveChangesAsync();
            }
        }
    }

    private async Task AssignPermissionsToAdminRoleAsync()
    {
        var adminRole = context.Entity<Role>().FirstOrDefault(r => r.Name == "Admin");
        if (adminRole == null) return;

        var permissions = context.Entity<Permission>().ToList();
        foreach (var permission in permissions.Where(permission => !context.Entity<RolePermission>().Any(rp => rp.RoleId == adminRole.Id && rp.PermissionId == permission.Id)))
        {
            context.Entity<RolePermission>().Add(new RolePermission
            {
                RoleId = adminRole.Id,
                PermissionId = permission.Id
            });
        }

        await context.SaveChangesAsync();
    }
}