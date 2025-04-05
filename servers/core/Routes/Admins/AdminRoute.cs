using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Routes;
using Core.Routes.Admins.Commands;
using Core.Routes.Admins.Dtos;
using Core.Routes.Admins.Queries;
using Core.Routes.Admins.Validators;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Shared.Infrastructure.Errors;
using Shared.Infrastructure.Rest;

namespace Core.Routes.Admins;

public class AdminRoute : IRoute
{
    public void MapRoutes(WebApplication app)
    {
        var group = app.MapGroup("/core/admins")
            .RequireAuthorization();

        group.MapGet("/", GetAllAdmins)
            .WithDescription("Получает всех админов");

        group.MapPost("/", CreateAdmin)
            .WithDescription("Создает нового админа");

        group.MapPost("/login", LoginAdmin)
            .WithDescription("Логинит админа")
            .AllowAnonymous();

        group.MapPost("/signout", SignoutAdmin)
            .WithDescription("Разлогинивает админа");

        group.MapGet("/me", GetAdminMe)
            .WithDescription("Получает свой профиль");

        group.MapGet("/{adminId}", GetAdminById)
            .WithDescription("Получает админа по айди");
    }

    public async Task<Results<Ok<AdminDto>, NotFound<ProblemDetails>>> GetAdminMe(IMediator mediator)
    {
        var query = new GetAdminMeQuery();
        var result = await mediator.Send(query);

        if (result.IsFailed)
        {
            return result.ToNotFoundProblem();
        }

        return TypedResults.Ok(result.Value);
    }

    public async Task<Results<Ok<AdminDto>, NotFound<ProblemDetails>>> GetAdminById(
        Guid adminId, IMediator mediator
    )
    {
        var query = new GetAdminByIdQuery()
        {
            AdminId = adminId,
        };

        var result = await mediator.Send(query);

        if (result.IsFailed)
        {
            return result.ToNotFoundProblem();
        }

        return TypedResults.Ok(result.Value);
    }

    public async Task<Ok<PagedList<AdminDto>>> GetAllAdmins([AsParameters] GetAllAdminsQuery query, IMediator mediator)
    {
        var admins = await mediator.Send(query);
        return TypedResults.Ok(admins);
    }

    public async Task<Results<Ok, BadRequest<ProblemDetails>, ValidationProblem>> LoginAdmin(
        LoginAdminCommand command, IValidator<LoginAdminCommand> validator, IMediator mediator
    )
    {
        if (validator.TryValidate(command, out var validation))
        {
            return validation.ToValidationProblem();
        }

        var result = await mediator.Send(command);

        if (result.IsFailed)
        {
            return result.ToBadRequestProblem();
        }

        return TypedResults.Ok();
    }

    public async Task<Results<Ok<AdminDto>, BadRequest<ProblemDetails>, ValidationProblem>> CreateAdmin(
        CreateAdminCommand command, IValidator<CreateAdminCommand> validator, IMediator mediator
    )
    {
        if (validator.TryValidate(command, out var validation))
        {
            return validation.ToValidationProblem();
        }

        var result = await mediator.Send(command);

        if (result.IsFailed)
        {
            return result.ToBadRequestProblem();
        }

        return TypedResults.Ok(result.Value);
    }

    public async Task<Ok> SignoutAdmin(IMediator mediator)
    {
        var command = new SignoutAdminCommand();
        await mediator.Send(command);
        return TypedResults.Ok();
    }
}