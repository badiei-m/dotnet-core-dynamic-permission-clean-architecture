using Application.Core;
using Cortex.Mediator.Queries;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Features.Auth.Permission;

public class PermissionList : IQuery<Result<List<PermissionTreeViewDto>>>
{
}

public class PermissionListHandler(AppDbContext context) : IQueryHandler<PermissionList, Result<List<PermissionTreeViewDto>>>
{
    public async Task<Result<List<PermissionTreeViewDto>>> Handle(PermissionList query, CancellationToken cancellationToken)
    {
        var permissions = await context.Entity<Domain.Entities.System.Permission>()
            .AsNoTracking()
            .Select(x=> new PermissionTreeViewDto
            {
                Id = x.Id,
                Title = x.Key,
                Child = new List<PermissionTreeViewDto>()
            })
            .ToListAsync(cancellationToken);
        
        var permissionLookup = permissions.ToDictionary(r => r.Id, r => r);

        foreach (var permission in permissions)
        {
            if (!permissionLookup.TryGetValue(permission.Id, out var currentRole)) continue;
            var dbPermission = await context.Entity<Domain.Entities.System.Permission>()
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == permission.Id,cancellationToken);

            if (dbPermission?.ParentId != null && permissionLookup.TryGetValue(dbPermission.ParentId.Value, out var parentRole))
            {
                parentRole.Child.Add(currentRole);
            }
        }
        var result = permissions.Where(r => !context.Entity<Domain.Entities.System.Role>()
            .Any(x => x.Id == r.Id && x.ParentId != null)).ToList();
        return Result<List<PermissionTreeViewDto>>.Success(result);
    }
}

public class PermissionTreeViewDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public List<PermissionTreeViewDto> Child { get; set; }
}