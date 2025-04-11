using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;

namespace Shared.Infrastructure.Extensions;

public interface IRoute
{
    void MapRoutes(WebApplication app);
}

public static class RouteExtensions
{
    public static void MapRoutes(this WebApplication app, Assembly assembly)
    {
        IEnumerable<IRoute> routes = assembly
            .GetTypes()
            .Where(t => t.IsAssignableTo(typeof(IRoute)) && !t.IsAbstract && !t.IsInterface)
            .Select(Activator.CreateInstance)
            .Cast<IRoute>();

        foreach (var route in routes)
        {
            route.MapRoutes(app);
        }
    }
}