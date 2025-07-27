using Application.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace InfraStructure.Services;

public class PermissionAuthorizationService : IPermissionAuthorizationService
{
    private readonly IUserRepository _userRepository;
    private readonly IMemoryCache _cache;

    public PermissionAuthorizationService(IUserRepository userRepository, IMemoryCache cache)
    {
        _userRepository = userRepository;
        _cache = cache;
    }

    public async Task<bool> HasPermissionAsync(string userId, string permission)
    {
        var cacheKey = $"permissions_{userId}";
        if (!_cache.TryGetValue(cacheKey, out List<string> permissions))
        {
            permissions = await _userRepository.GetUserPermissionsAsync(userId);
            _cache.Set(cacheKey, permissions, TimeSpan.FromMinutes(30));
        }
        return permissions.Contains(permission, StringComparer.OrdinalIgnoreCase);
    }
}