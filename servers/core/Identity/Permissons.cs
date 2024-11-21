using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Identity;

public class Permission
{
    public string Claim { get; private set; }
    public string Policy => Claim + "Policy";

    public Permission(string name)
    {
        Claim = name;
    }
}

public static class RouteHandlerBuilderExtensions
{
    public static RouteHandlerBuilder AddPermission(this RouteHandlerBuilder handler, Permission permission)
    {
        handler.RequireAuthorization(permission.Policy);
        return handler;
    }
}

public static class RouteGroupBuilderExtensions
{
    public static RouteGroupBuilder AddPermission(this RouteGroupBuilder group, Permission permission)
    {
        group.RequireAuthorization(permission.Policy);
        return group;
    }
}

public static class Permissions
{
    public static Permission View => new Permission("View");
    public static Permission CreateAdmins => new Permission("CreateAdmins");
    public static Permission SendMessages => new Permission("SendMessages");
    public static Permission SendNotifications => new Permission("SendNotifications");
    public static Permission ManipulateStudents => new Permission("ManipulateStudents");
    public static Permission UploadFiles => new Permission("UploadFiles");

    public static List<Permission>
        All => [View, CreateAdmins, SendMessages, SendNotifications, ManipulateStudents, UploadFiles];
}

public static class AuthorizationExtensions
{
    public static void AddPermissions(this IServiceCollection services)
    {
        Action<AuthorizationOptions> authOptions = options =>
        {
            foreach (var permission in Permissions.All)
            {
                options.AddPolicy(permission.Policy, policy => policy.RequireClaim(permission.Claim));
            }
        };

        services.AddAuthorization(authOptions);
    }
}
