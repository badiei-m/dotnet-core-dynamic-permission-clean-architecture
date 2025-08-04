using Application.Core;
using Application.DTOs;
using Application.Interfaces;
using Cortex.Mediator.Queries;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Features.System.User;

public class GetCurrentUser : IQuery<Result<UserDto>>
{
    public string UserId { get; set; }
}
public class GetCurrentUserHandler(AppDbContext context,ITokenService tokenService) : IQueryHandler<GetCurrentUser, Result<UserDto>>
{
    public async Task<Result<UserDto>> Handle(GetCurrentUser query, CancellationToken cancellationToken)
    {
        var user = await context.Entity<Domain.Entities.System.User>()
            .Include(x=>x.UserRole)
            .FirstOrDefaultAsync(c=>c.Id.ToString() == query.UserId, cancellationToken: cancellationToken);
        return Result<UserDto>.Success(CreateUserObject(user));
    }
    
    private UserDto CreateUserObject(Domain.Entities.System.User user)
    {
        return new UserDto
        {
            Username = user.UserName,
            Token = tokenService.CreateToken(user)
        };
    }
}