using System.Security.Claims;

namespace API.Services;

public class UserService(IHttpContextAccessor accessor)
{
    public string GetCurrentUserId()
    {
        return accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}