namespace Application.Interfaces;

public interface IUserRepository
{
    Task<List<string>> GetUserPermissionsAsync(string userId);
}
