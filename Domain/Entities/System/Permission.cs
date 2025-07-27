namespace Domain.Entities.System;

public class Permission
{
    public Guid Id { get; set; }
    public string Key { get; set; } = "";
    public string Description { get; set; } = "";
    public Guid? ParentId { get; set; }
    public Permission Parent { get; set; }
    public ICollection<RolePermission> RolePermissions { get; set; }
    
}