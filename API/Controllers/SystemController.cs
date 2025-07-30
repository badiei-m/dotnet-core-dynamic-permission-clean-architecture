using API.Authorization;
using Application.Core;
using Application.Features.Permission;
using Application.Features.Role;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[MustHavePermission]
[Route("api/[controller]/[action]")]
public class SystemController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetRoles()
    {
        var roleList = await Mediator
            .SendQueryAsync<RoleList, Result<List<RoleTreeViewDto>>>(new RoleList{});
        return HandleResult(roleList);
    }
    
    [HttpPost]
    [ParentAction("System", "GetRoles")]
    public async Task<IActionResult> RegisterRole([FromBody]RegisterRole request)
    {
        var userId = await Mediator.SendCommandAsync<RegisterRole, Result<Guid?>>(request);
        return HandleResult(userId);
    }

    [HttpGet]
    [ParentAction("System", "GetRoles")]
    public async Task<IActionResult> GetPermissions()
    {
        var permissionList = await Mediator.SendQueryAsync<PermissionList, Result<List<PermissionTreeViewDto>>>(
            new PermissionList{});
        return HandleResult(permissionList);
    }
}