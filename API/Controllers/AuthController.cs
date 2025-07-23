using API.DTOs;
using API.Services;
using Domain.Entities.System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace API.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/[controller]/[action]")]
public class AuthController(AppDbContext context,TokenService tokenService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<UserDto>> Login([FromBody] LoginDto request)
    {
        var user = await context.Entity<User>()
            .Include(u => u.UserRole)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.UserName == request.Username);

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return Unauthorized("Invalid credentials");
        
        return CreateUserObject(user);
    }
    
    [HttpPost]
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
        if(await context.Entity<User>().AnyAsync(x=>x.UserName == registerDto.UserName))
        {
            ModelState.AddModelError("username","Username is already taken");
            return ValidationProblem();
        }

        if(await context.Entity<User>().AnyAsync(x=>x.Email == registerDto.Email))
        {
            ModelState.AddModelError("email","Email is already taken");
            return ValidationProblem();
        }
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);
        var user = new User
        {
            Email = registerDto.Email,
            UserName = registerDto.UserName,
            DisplayName = registerDto.DisplayName,
            PasswordHash = passwordHash
        };
        context.Entity<User>().Add(user);
        await context.SaveChangesAsync();

        var commonRole = await context.Entity<Role>().FirstOrDefaultAsync(x => x.Name == "Common");

        if (commonRole == null) return CreateUserObject(user);
        var role = new UserRole
        {
            RoleId = commonRole.Id,
            UserId = user.Id
        };
        context.Entity<UserRole>().Add(role);
        await context.SaveChangesAsync();

        return CreateUserObject(user);
    }
    
    private UserDto CreateUserObject(User user)
    {
        return new UserDto
        {
            Username = user.UserName,
            Token = tokenService.CreateToken(user)
        };
    }
}


