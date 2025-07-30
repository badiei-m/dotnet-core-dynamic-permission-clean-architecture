using API.Authorization;
using Application.Features.Permission;
using Application.Features.Role;
using Cortex.Mediator;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[MustHavePermission]
[Route("api/[controller]/[action]")]
public class SystemController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<RoleTreeViewDto>>> GetRoles()
    {
        var roleList = await mediator.SendQueryAsync<RoleList, List<RoleTreeViewDto>>(
            new RoleList{});
        return roleList;
    }
    
    [HttpPost]
    [ParentAction("System", "GetRoles")]
    public async Task<ActionResult<string>> RegisterRole([FromBody]RegisterRole request)
    {
        var userId = await mediator.SendCommandAsync<RegisterRole, Guid?>(request);
        if (userId == null)
        {
            return Unauthorized("Invalid credentials");
        }
        return Ok(userId);    
    }
    

    [HttpGet]
    [ParentAction("System", "GetRoles")]
    public async Task<ActionResult<List<PermissionTreeViewDto>>> GetPermissions()
    {
        var permissionList = await mediator.SendQueryAsync<PermissionList, List<PermissionTreeViewDto>>(
            new PermissionList{});
        return permissionList;
    }
}