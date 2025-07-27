using System.Reflection;
using API.Authorization;
using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace API.Services;

public class PermissionScanner : IPermissionScanner
{
    public IEnumerable<PermissionDto> GetPermissionNames()
    {
        var permissions = new List<PermissionDto>();

        // Get all controllers in the executing assembly
        var controllerTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(type => typeof(ControllerBase).IsAssignableFrom(type)
                           && !type.IsAbstract
                           && !type.GetCustomAttributes(typeof(AllowAnonymousAttribute), true).Any());

        // Collect all valid actions first to validate parents later
        var allActions = new Dictionary<string, bool>(); // Key: "controller.action", Value: IsValid (not AllowAnonymous)

        foreach (var controllerType in controllerTypes)
        {
            var controllerName = controllerType.Name.Replace("Controller", "", StringComparison.OrdinalIgnoreCase).ToLower();

            var actionMethods = controllerType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(m => !m.IsSpecialName
                            && m.GetCustomAttributes(typeof(HttpMethodAttribute), true).Any()
                            && !m.GetCustomAttributes(typeof(AllowAnonymousAttribute), true).Any());

            foreach (var method in actionMethods)
            {
                var actionName = method.Name.ToLower();
                var permission = $"{controllerName}.{actionName}";
                allActions[permission] = true; // Mark as valid action
            }
        }

        foreach (var controllerType in controllerTypes)
        {
            var controllerName = controllerType.Name.Replace("Controller", "", StringComparison.OrdinalIgnoreCase).ToLower();

            var actionMethods = controllerType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(m => !m.IsSpecialName
                            && m.GetCustomAttributes(typeof(HttpMethodAttribute), true).Any()
                            && !m.GetCustomAttributes(typeof(AllowAnonymousAttribute), true).Any());

            foreach (var method in actionMethods)
            {
                var actionName = method.Name.ToLower();
                var permission = $"{controllerName}.{actionName}";

                // Check for ParentActionAttribute
                var parentAttr = method.GetCustomAttribute<ParentActionAttribute>(true);
                string? parentPermission = null;

                if (parentAttr != null)
                {
                    parentPermission = $"{parentAttr.ParentController}.{parentAttr.ParentAction}";
                    if (!allActions.ContainsKey(parentPermission))
                        continue;
                }

                var newPermission = new PermissionDto
                {
                    Permission = permission,
                    ParentPermission = parentPermission,
                };
                permissions.Add(newPermission);
            }
        }

        return permissions;
    }
}