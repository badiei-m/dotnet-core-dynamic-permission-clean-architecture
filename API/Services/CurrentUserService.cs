using System.Security.Claims;
using API.Interfaces;

namespace API.Services;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public string? UserId =>
        httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    public string? UserName =>
        httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

    public string? Roles =>
        httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value;
}