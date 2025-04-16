using Core.Features.Admins.Commands;
using Core.Features.Admins.Queries;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Shared.Infrastructure.Extensions;

namespace Core.Features.Admins;

public class AdminRoute : IRoute
{
    public void MapRoutes(WebApplication app)
    {
        var group = app.MapGroup("/core/admins")
            .WithTags("Админы")
            .RequireAuthorization();

        group.MapGet("/", GetAllAdminsQuery.Handle)
            .WithDescription("Получает всех админов");

        group.MapPost("/", CreateAdminCommand.Handle)
            .WithDescription("Создает нового админа");

        group.MapPost("/login", LoginAdminCommand.Handle)
            .WithDescription("Логинит админа")
            .AllowAnonymous();

        group.MapPost("/signout", SignoutAdminCommand.Handle)
            .WithDescription("Разлогинивает админа");

        group.MapGet("/me", GetAdminMeQuery.Handle)
            .WithDescription("Получает свой профиль");

        group.MapGet("/{adminId}", GetAdminByIdQuery.Handle)
            .WithDescription("Получает админа по айди");
    }
}