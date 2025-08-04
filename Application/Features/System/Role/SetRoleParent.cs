using Application.Core;
using Cortex.Mediator.Commands;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Features.System.Role;

public class SetRoleParent : ICommand<Result<string>>
{
    public Guid RoleId { get; set; }
    public Guid ParentId { get; set; }
}

public class SetRoleParentHandler(AppDbContext context) : ICommandHandler<SetRoleParent, Result<string>>
{
    public async Task<Result<string>> Handle(SetRoleParent command, CancellationToken cancellationToken)
    {
        var role = await context.Entity<Domain.Entities.System.Role>()
            .FirstOrDefaultAsync(x=>x.Id == command.RoleId,cancellationToken);
        if(role == null)
            return Result<string>.Failure("Role not found");
        
        var parent = await context.Entity<Domain.Entities.System.Role>()
            .FirstOrDefaultAsync(x=>x.Id == command.ParentId,cancellationToken);
        if(parent == null)
            return Result<string>.Failure("Parent Role not found");

        var hasChild = await context.Entity<Domain.Entities.System.Role>()
            .AnyAsync(x => x.ParentId == command.RoleId, cancellationToken);
        if(hasChild)
            return Result<string>.Failure("You can't change the parent of a role that has child");
        
        role.ParentId = command.ParentId;
        await context.SaveChangesAsync(cancellationToken);
        return Result<string>.Success("Role updated");
    }
}