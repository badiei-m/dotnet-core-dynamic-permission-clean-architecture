using Cortex.Mediator.Commands;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Features.Role;

public class RegisterRole : ICommand<Guid?>
{
    public string Name { get; set; }
    public string Parent { get; set; }
}

public class RegisterRoleHandler(AppDbContext context) : ICommandHandler<RegisterRole, Guid?>
{
    public async Task<Guid?> Handle(RegisterRole command, CancellationToken cancellationToken)
    {
        var exist = await context.Entity<Domain.Entities.System.Role>()
            .AnyAsync(x => x.Name == command.Name,cancellationToken);
        if (!exist)
            return null; 
        
        var role = new Domain.Entities.System.Role
        {
            Name = command.Name,
            ParentId = new Guid(command.Parent)
        };
        context.Entity<Domain.Entities.System.Role>().Add(role);
        await context.SaveChangesAsync(cancellationToken);
        
        return role.Id;
    }
}