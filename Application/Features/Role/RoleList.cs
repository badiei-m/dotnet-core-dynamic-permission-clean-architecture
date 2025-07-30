using Application.Core;
using Cortex.Mediator.Queries;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Features.Role;
public class RoleList : IQuery<Result<List<RoleTreeViewDto>>>
{
}

public class RoleListHandler(AppDbContext context) : IQueryHandler<RoleList,Result<List<RoleTreeViewDto>>>
{
    public async Task<Result<List<RoleTreeViewDto>>> Handle(RoleList command, CancellationToken cancellationToken)
    {
        var roles = await context.Entity<Domain.Entities.System.Role>()
            .AsNoTracking()
            .Select(r => new RoleTreeViewDto
            {
                Id = r.Id,
                Title = r.Name,
                ChildList = new List<RoleTreeViewDto>() 
            })
            .ToListAsync(cancellationToken);

        var roleLookup = roles.ToDictionary(r => r.Id, r => r);

        foreach (var role in roles)
        {
            if (!roleLookup.TryGetValue(role.Id, out var currentRole)) continue;
            var dbRole = await context.Entity<Domain.Entities.System.Role>()
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == role.Id,cancellationToken);

            if (dbRole?.ParentId != null && roleLookup.TryGetValue(dbRole.ParentId.Value, out var parentRole))
            {
                parentRole.ChildList.Add(currentRole);
            }
        }
        var result = roles
            .Where(r => 
                !context.Entity<Domain.Entities.System.Role>()
                    .Any(dbRole => dbRole.Id == r.Id && dbRole.ParentId != null))
            .ToList();
        return Result<List<RoleTreeViewDto>>.Success(result);
    }
}

public class RoleTreeViewDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public List<RoleTreeViewDto> ChildList { get; set; }
}