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
        await context.Database.EnsureCreatedAsync();
        await SeedPermissionsFromControllersAsync();
        var roleId = await SeedAdminRoleAsync();
        await SeedAdminUserAsync();
        var commonRole = await SeedCommonRoleAsync(roleId);
        await SeedCommonUserAsync(commonRole);
        await AssignPermissionsToAdminRoleAsync();
    }

    private async Task SeedPermissionsFromControllersAsync()
    {
        var permissions = permissionScanner.GetPermissionNames();
        var permissionsToAdd = new List<Permission>();

        foreach (var item in permissions)
        {
            if (context.Entity<Permission>().Any(p => p.Key == item.Permission)) continue;
            Guid? parentId = null;
            if (item.ParentPermission != null)
            {
                var parent  = await context.Entity<Permission>()
                    .FirstOrDefaultAsync(x => x.Key == item.ParentPermission);
                parentId = parent.Id;
            }
            permissionsToAdd.Add(new Permission
            {
                Key = item.Permission,
                ParentId = parentId,
                Description =
                    $"Permission to {item.Permission.Split('.').Last()} {item.Permission.Split('.').First()}"
            });
        }

        if (permissionsToAdd.Any())
        {
            context.Entity<Permission>().AddRange(permissionsToAdd);
            await context.SaveChangesAsync();
        }
    }

    private async Task<Guid> SeedAdminRoleAsync()
    {
        const string adminRoleName = "Admin";
        var admin =await context.Entity<Role>().FirstOrDefaultAsync(r => r.Name == adminRoleName);
        Guid roleId;
        if (admin == null)
        {
            var adminRole = new Role { Name = adminRoleName };
            context.Entity<Role>().Add(adminRole);
            await context.SaveChangesAsync();
            roleId = adminRole.Id;
        }
        else
            roleId = admin.Id;
        
        return roleId;
    }

    private async Task<Guid> SeedCommonRoleAsync(Guid parentId)
    {
        const string adminRoleName = "Common";
        var admin =await context.Entity<Role>().FirstOrDefaultAsync(r => r.Name == adminRoleName);

        Guid roleId;
        if (admin == null)
        {
            var adminRole = new Role
            {
                Name = adminRoleName,
                ParentId = parentId
            };
            context.Entity<Role>().Add(adminRole);
            await context.SaveChangesAsync();
            roleId = adminRole.Id;
        }
        else
            roleId = admin.Id;

        return roleId;
    }

    private async Task SeedCommonUserAsync(Guid roleId)
    {
        const string username = "common";
        const string password = "commonPassword";
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

            var role = new UserRole
            {
                RoleId = roleId,
                UserId = permissionTesterUser.Id
            };
            
            context.Entity<UserRole>().Add(role);
            await context.SaveChangesAsync();
        }
    }
    private async Task SeedAdminUserAsync()
    {
        const string adminUsername = "admin";
        const string adminPassword = "Admin@123!";
        
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

            if (!context.Entity<User>().Any(u => u.Id == adminUser.Id))
            {
                context.Entity<User>().Add(new User { Id = adminUser.Id, UserName = adminUsername });
                await context.SaveChangesAsync();
            }

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