using API.Authorization;
using API.Services;
using Application.Core;
using Application.DTOs;
using Application.Features.System.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]/[action]")]
public class AuthController(UserService userService) :
    BaseApiController
{
    [HttpPost,AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] Login request)
    {
        var userId = await Mediator.SendCommandAsync<Login, Result<UserDto>>(request);
        return HandleResult(userId);
    }
    
    [HttpPost,AllowAnonymous]
    public async Task<ActionResult<UserDto>> Register([FromBody]RegisterUser request)
    {
        var user = await Mediator.SendCommandAsync<RegisterUser, Result<UserDto>>(request);
        return HandleResult(user);
    }

    [HttpGet,MustHavePermission]
    public async Task<ActionResult<UserDto>> GetCurrentUser()
    {
        var userId = userService.GetCurrentUserId();
        var user = await Mediator.SendQueryAsync<GetCurrentUser, Result<UserDto>>(new GetCurrentUser{UserId = userId});
        return HandleResult(user);
    }
}


