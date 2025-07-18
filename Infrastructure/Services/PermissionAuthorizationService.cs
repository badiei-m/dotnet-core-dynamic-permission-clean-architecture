using Application.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace InfraStructure.Services;

public class PermissionAuthorizationService : IPermissionAuthorizationService
{
    private readonly IPermissionRepository _permissionRepository;
    private readonly IMemoryCache _cache;

    public PermissionAuthorizationService(IPermissionRepository permissionRepository, IMemoryCache cache)
    {
        _permissionRepository = permissionRepository;
        _cache = cache;
    }

    public async Task<bool> HasPermissionAsync(string userId, string permission)
    {
        var cacheKey = $"permissions_{userId}";
        if (!_cache.TryGetValue(cacheKey, out List<string> permissions))
        {
            permissions = await _permissionRepository.GetUserPermissionsAsync(userId);
            _cache.Set(cacheKey, permissions, TimeSpan.FromMinutes(30));
        }
        return permissions.Contains(permission, StringComparer.OrdinalIgnoreCase);
    }
}