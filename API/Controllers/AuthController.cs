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
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Username == request.Username);

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return Unauthorized("Invalid credentials");
        
        return CreateUserObject(user);
    }
    
    private UserDto CreateUserObject(User user)
    {
        return new UserDto
        {
            Username = user.Username,
            Token = tokenService.CreateToken(user)
        };
    }
}


