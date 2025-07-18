using System.Reflection;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace API.Services;

public class PermissionScanner : IPermissionScanner
{
    public IEnumerable<string> GetPermissionNames()
    {
        var permissions = new List<string>();

        // Get all controllers in the executing assembly
        var controllerTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(type => typeof(ControllerBase).IsAssignableFrom(type) && !type.IsAbstract);

        foreach (var controllerType in controllerTypes)
        {
            // Extract controller name (remove "Controller" suffix)
            var controllerName = controllerType.Name.Replace("Controller", "", StringComparison.OrdinalIgnoreCase).ToLower();

            // Get public action methods with HTTP attributes and exclude [AllowAnonymous]
            var actionMethods = controllerType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(m => !m.IsSpecialName && // Exclude getters/setters
                            m.GetCustomAttributes(typeof(HttpMethodAttribute), true).Any() && // HTTP methods (Get, Post, etc.)
                            !m.GetCustomAttributes(typeof(AllowAnonymousAttribute), true).Any()); // Exclude [AllowAnonymous]

            permissions.AddRange(actionMethods.Select(method => method.Name.ToLower()).Select(actionName => $"{controllerName}.{actionName}"));
        }

        return permissions;
    }
}