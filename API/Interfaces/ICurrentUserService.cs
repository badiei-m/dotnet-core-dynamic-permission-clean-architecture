namespace API.Interfaces;

public interface ICurrentUserService
{
    string? UserId { get; }
    string? UserName { get; } 
    string? Roles { get; }
}