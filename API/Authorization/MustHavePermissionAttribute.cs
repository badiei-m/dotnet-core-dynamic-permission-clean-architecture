using System.Security.Claims;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Authorization;

public class MustHavePermissionAttribute(string permission = null) : AuthorizeAttribute, IAsyncAuthorizationFilter
{
    public string Permission { get; } = permission;

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        // Skip authorization if action or controller has [AllowAnonymous]
        if (context.ActionDescriptor.EndpointMetadata.Any(em => em is AllowAnonymousAttribute))
        {
            return;
        }

        // Ensure user is authenticated
        if (!context.HttpContext.User.Identity.IsAuthenticated)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        // Resolve IAuthorizationService from DI
        var authorizationService =
            context.HttpContext.RequestServices.GetService(typeof(IPermissionAuthorizationService)) as IPermissionAuthorizationService;
        if (authorizationService == null)
        {
            context.Result = new StatusCodeResult(500); // Internal server error
            return;
        }

        // Get user ID from claims
        var userId = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        // Determine permission to check
        string requiredPermission = Permission;
        if (string.IsNullOrEmpty(requiredPermission))
        {
            // Auto-generate permission from controller and action (e.g., "orders.read")
            var controller = context.ActionDescriptor.RouteValues["controller"]?.ToLower();
            var action = context.ActionDescriptor.RouteValues["action"]?.ToLower();
            requiredPermission = $"{controller}.{action}";
        }

        // Check permission
        bool hasPermission = await authorizationService.HasPermissionAsync(userId, requiredPermission);
        if (!hasPermission)
        {
            context.Result = new ForbidResult();
        }
    }
}