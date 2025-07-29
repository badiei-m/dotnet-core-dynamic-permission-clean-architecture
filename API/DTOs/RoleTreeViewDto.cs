namespace API.DTOs;

public class RoleTreeViewDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = "";
    public List<RoleTreeViewDto> Child { get; set; }
}