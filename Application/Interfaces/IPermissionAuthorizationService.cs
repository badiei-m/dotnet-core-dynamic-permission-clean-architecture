using Microsoft.Extensions.Caching.Memory;

namespace Application.Interfaces;

public interface IPermissionAuthorizationService
{
    Task<bool> HasPermissionAsync(string userId, string permission);
}
