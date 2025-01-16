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
    public string Name { get; private set; }

    public Permission(string claim, string name)
    {
        Claim = claim;
        Name = name;
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
    public static Permission View => new Permission("View", "Просмотр");
    public static Permission CreateAdmins => new Permission("CreateAdmins", "Создание админов");
    public static Permission SendMessages => new Permission("SendMessages", "Отправка сообщений");
    public static Permission SendNotifications => new Permission("SendNotifications", "Отправка рассылок");
    public static Permission ManipulateStudents => new Permission("ManipulateStudents", "Редактирование студентов");
    public static Permission UploadFiles => new Permission("UploadFiles", "Загрузка файлов");

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
