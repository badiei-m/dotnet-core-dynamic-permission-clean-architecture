namespace Application.Interfaces;

public interface IPermissionRepository
{
    Task<List<string>> GetUserPermissionsAsync(string userId);
}
