using API.Authorization;
using API.DTOs;
using Domain.Entities.System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace API.Controllers;

[ApiController]
[MustHavePermission]
[Route("api/[controller]/[action]")]
public class SystemController(AppDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<RoleTreeViewDto>>> GetRoles()
    {
        var roles = await context.Entity<Role>()
            .AsNoTracking()
            .Select(r => new RoleTreeViewDto
            {
                Id = r.Id,
                Title = r.Name,
                Child = new List<RoleTreeViewDto>() 
            })
            .ToListAsync();

        var roleLookup = roles.ToDictionary(r => r.Id, r => r);

        foreach (var role in roles)
        {
            if (!roleLookup.TryGetValue(role.Id, out var currentRole)) continue;
            var dbRole = await context.Entity<Role>()
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == role.Id);

            if (dbRole?.ParentId != null && roleLookup.TryGetValue(dbRole.ParentId.Value, out var parentRole))
            {
                parentRole.Child.Add(currentRole);
            }
        }
        return roles.Where(r => !context.Entity<Role>().Any(dbRole => dbRole.Id == r.Id && dbRole.ParentId != null)).ToList();
    }
    
    [HttpPost]
    [ParentAction("System", "GetRoles")]
    public async Task<ActionResult<string>> RegisterRole(RegisterRoleDto request)
    {
        var exist = await context.Entity<Role>().AnyAsync(x => x.Name == request.Name);
        if (!exist)
            return Unauthorized("Invalid credentials"); 
        
        var role = new Role
        {
            Name = request.Name,
            ParentId = new Guid(request.Parent)
        };
        context.Entity<Role>().Add(role);
        await context.SaveChangesAsync();
        
        return Ok(role.Id);
    }
    

    [HttpGet]
    public async Task<ActionResult<List<PermissionTreeViewDto>>> GetPermissions()
    {
        var permissions = await context.Entity<Permission>()
            .AsNoTracking()
            .Select(x=> new PermissionTreeViewDto
            {
                Id = x.Id,
                Title = x.Key,
                Child = new List<PermissionTreeViewDto>()
            })
            .ToListAsync();
        
        var permissionLookup = permissions.ToDictionary(r => r.Id, r => r);

        foreach (var permission in permissions)
        {
            if (!permissionLookup.TryGetValue(permission.Id, out var currentRole)) continue;
            var dbPermission = await context.Entity<Permission>()
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == permission.Id);

            if (dbPermission?.ParentId != null && permissionLookup.TryGetValue(dbPermission.ParentId.Value, out var parentRole))
            {
                parentRole.Child.Add(currentRole);
            }
        }
        return permissions.Where(r => !context.Entity<Role>()
            .Any(x => x.Id == r.Id && x.ParentId != null)).ToList();
    }
}