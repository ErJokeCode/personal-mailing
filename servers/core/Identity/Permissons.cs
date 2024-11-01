using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Identity;

public static class Permissions
{
    public static string View => "View";
    public static string ViewPolicy => View + "Policy";

    public static string UploadFiles => "UploadFiles";
    public static string UploadFilesPolicy => UploadFiles + "Policy";

    public static string SendNotifications => "SendNotifications";
    public static string SendNotificationsPolicy => SendNotifications + "Policy";

    public static string CreateAdmins => "CreateAdmins";
    public static string CreateAdminsPolicy => CreateAdmins + "Policy";

    public static List<string> All => [View, UploadFiles, SendNotifications, CreateAdmins];
}

public static class AuthorizationExtensions
{
    public static void AddPermissions(this IServiceCollection services)
    {
        Action<AuthorizationOptions> authOptions = options =>
        {
            foreach (var permission in Permissions.All)
            {
                options.AddPolicy($"{permission}Policy", policy => policy.RequireClaim($"{permission}"));
            }
        };

        services.AddAuthorization(authOptions);
    }
}
