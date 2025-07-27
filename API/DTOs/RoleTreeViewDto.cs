namespace API.DTOs;

public class RoleTreeViewDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public List<RoleTreeViewDto> Child { get; set; } = new List<RoleTreeViewDto>();
}