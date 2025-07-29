namespace API.DTOs;

public class PermissionTreeViewDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public List<PermissionTreeViewDto> Child { get; set; }
}