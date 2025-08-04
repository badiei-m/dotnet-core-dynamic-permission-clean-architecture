using System.ComponentModel.DataAnnotations;
using Application.Core;
using Application.DTOs;
using Application.Interfaces;
using Cortex.Mediator.Commands;
using Domain.Entities.System;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Features.System.User;

public class RegisterUser : ICommand<Result<UserDto>>
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    [RegularExpression("(?=.*\\d)(?=.*[a-z])(?=.*[A-Z]).{4,8}$",ErrorMessage ="Password Must Be Complex.")]
    public string Password { get; set; }
    [Required]
    public string DisplayName { get; set; }
    [Required]
    public string UserName { get; set; }
}

public class RegisterUserHandler(AppDbContext context,ITokenService tokenService) :
    ICommandHandler<RegisterUser,Result<UserDto>>
{
    public async Task<Result<UserDto>> Handle(RegisterUser command, CancellationToken cancellationToken)
    {
        var isUserNameTaken = await context.Entity<Domain.Entities.System.User>()
            .AnyAsync(x => x.UserName == command.UserName, cancellationToken: cancellationToken);
        if(isUserNameTaken)
            return Result<UserDto>.Failure("Username is already taken");

        var isEmailTaken = await context.Entity<Domain.Entities.System.User>()
            .AnyAsync(x => x.Email == command.Email, cancellationToken); 
        if(isEmailTaken)
            return Result<UserDto>.Failure("Email is already taken");

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(command.Password);
        var user = new Domain.Entities.System.User
        {
            Email = command.Email,
            UserName = command.UserName,
            DisplayName = command.DisplayName,
            PasswordHash = passwordHash
        };
        context.Entity<Domain.Entities.System.User>().Add(user);
        await context.SaveChangesAsync(cancellationToken);

        var commonRole = await context.Entity<Domain.Entities.System.Role>()
            .FirstOrDefaultAsync(x => x.Name == "Common", cancellationToken: cancellationToken);

        if (commonRole != null)
        {
            var role = new UserRole
            {
                RoleId = commonRole.Id,
                UserId = user.Id
            };
            context.Entity<UserRole>().Add(role);
            await context.SaveChangesAsync(cancellationToken);
        }

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