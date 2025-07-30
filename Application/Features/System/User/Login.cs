using Application.Core;
using Application.DTOs;
using Application.Interfaces;
using Cortex.Mediator.Commands;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Features.System.User;

public class Login:ICommand<Result<UserDto>>
{
    public string Username { get; set; }
    public string Password { get; set; }
}

public class LoginHandler(AppDbContext context,ITokenService tokenService) : ICommandHandler<Login, Result<UserDto>>
{
    public async Task<Result<UserDto>> Handle(Login command, CancellationToken cancellationToken)
    {
        var user = await context.Entity<Domain.Entities.System.User>()
            .Include(u => u.UserRole)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.UserName == command.Username,cancellationToken);

        if (user == null || !BCrypt.Net.BCrypt.Verify(command.Password, user.PasswordHash))
            return Result<UserDto>.Failure("Invalid credentials");
        
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

