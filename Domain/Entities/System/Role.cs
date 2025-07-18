namespace Domain.Entities.System;

public class Role
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";

    public ICollection<UserRole> UserRoles { get; set; }
    public ICollection<RolePermission> RolePermissions { get; set; }
}