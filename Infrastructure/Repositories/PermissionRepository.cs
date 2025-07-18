using Application.Interfaces;
using Domain.Entities.System;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace InfraStructure.Repositories;

public class PermissionRepository(AppDbContext context) : IPermissionRepository
{
    public async Task<List<string>> GetUserPermissionsAsync(string userId)
    {
        return await context.Entity<UserRole>()
            .Where(ur => ur.UserId.ToString() == userId)
            .Join(context.Entity<RolePermission>(),
                ur => ur.RoleId,
                rp => rp.RoleId,
                (ur, rp) => rp.PermissionId)
            .Join(context.Entity<Permission>(),
                rp => rp,
                p => p.Id,
                (rp, p) => p.Key)
            .Distinct()
            .ToListAsync();
    }
    
}