using Application.Core;
using Cortex.Mediator.Commands;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Features.Role;

public class RegisterRole : ICommand<Result<Guid?>>
{
    public string Name { get; set; }
    public string Parent { get; set; }
}

public class RegisterRoleHandler(AppDbContext context) : ICommandHandler<RegisterRole, Result<Guid?>>
{
    public async Task<Result<Guid?>> Handle(RegisterRole command, CancellationToken cancellationToken)
    {
        var exist = await context.Entity<Domain.Entities.System.Role>()
            .AnyAsync(x => x.Name == command.Name,cancellationToken);
        if (!exist)
            Result<Guid?>.Failure("Role does not exist.");
        
        var role = new Domain.Entities.System.Role
        {
            Name = command.Name,
            ParentId = new Guid(command.Parent)
        };
        context.Entity<Domain.Entities.System.Role>().Add(role);
        await context.SaveChangesAsync(cancellationToken);
        
        return Result<Guid?>.Success(role.Id);
    }
}