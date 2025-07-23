namespace Domain.Entities.System;

public class User
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = "";
    public string Email { get; set; }
    public string DisplayName { get; set; }
    public string PasswordHash { get; set; } = "";

    public UserRole UserRole { get; set; }
}