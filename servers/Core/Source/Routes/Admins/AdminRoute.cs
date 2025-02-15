using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Infrastructure.Errors;
using Core.Routes;
using Core.Routes.Admins.Commands;
using Core.Routes.Admins.Dtos;
using Core.Routes.Admins.Queries;
using Core.Routes.Admins.Validators;
using Core.Routes.Notifications.Dtos;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Core.Routes.Admins;

public class AdminRoute : IRoute
{
    public void MapRoutes(WebApplication app)
    {
        var group = app.MapGroup("/core/admins")
            .RequireAuthorization();

        group.MapPost("/", CreateAdmin)
            .WithDescription("Creates a new admin");

        group.MapPost("/login", LoginAdmin)
            .WithDescription("Logs in the admin by issuing a cookie")
            .AllowAnonymous();

        group.MapPost("/signout", SignoutAdmin)
            .WithDescription("Signs out the admin by retrieving the cookie");

        group.MapGet("/", GetAllAdmins)
            .WithDescription("Gets all admins");

        group.MapGet("/me", GetAdminMe)
            .WithDescription("Gets an admin by cookie");

        group.MapGet("/groups", GetAllGroups)
            .WithDescription("Gets all group assignments");

        group.MapGet("/{adminId}", GetAdminById)
            .WithDescription("Gets an admin by id");

        group.MapGet("/{adminId}/notifications", GetAdminNotificatioins)
            .WithDescription("Gets a compact version of all notifications of an admin");

        // TODO rewrite using a separate group table, with metadata like an admin assigned to this group
        // Overall number of students, number of authed students, and we can work with that
        group.MapPatch("/{adminId}/groups", AssignGroups)
            .WithDescription("Assigns groups to the admin");
    }

    private async Task<Ok<IEnumerable<GroupAssignmentDto>>> GetAllGroups(IMediator mediator)
    {
        var query = new GetAllGroupsQuery();

        var groups = await mediator.Send(query);

        return TypedResults.Ok(groups);
    }

    public async Task<Results<NoContent, NotFound<ProblemDetails>, ValidationProblem>> AssignGroups(
        Guid adminId, ICollection<int> groups, IValidator<GetAdminByIdQuery> idValidator, IValidator<AssignGroupsCommand> assignValidator, IMediator mediator
    )
    {
        var adminQuery = new GetAdminByIdQuery()
        {
            AdminId = adminId
        };

        var idValidation = await idValidator.ValidateAsync(adminQuery);

        if (!idValidation.IsValid)
        {
            return idValidation.ToValidationProblem();
        }

        var adminResult = await mediator.Send(adminQuery);

        if (adminResult.IsFailed)
        {
            return adminResult.ToNotFoundProblem();
        }

        var command = new AssignGroupsCommand()
        {
            AdminId = adminId,
            GroupIds = groups,
        };

        var assignValidation = await assignValidator.ValidateAsync(command);

        if (!assignValidation.IsValid)
        {
            return assignValidation.ToValidationProblem();
        }

        var result = await mediator.Send(command);

        if (result.IsFailed)
        {
            return result.ToNotFoundProblem();
        }

        return TypedResults.NoContent();
    }

    public async Task<Results<Ok<IEnumerable<NotificationDto>>, NotFound<ProblemDetails>, ValidationProblem>> GetAdminNotificatioins(
        Guid adminId, IValidator<GetAdminByIdQuery> validator, IMediator mediator
    )
    {
        var adminQuery = new GetAdminByIdQuery()
        {
            AdminId = adminId
        };

        var validationResult = await validator.ValidateAsync(adminQuery);

        if (!validationResult.IsValid)
        {
            return validationResult.ToValidationProblem();
        }

        var adminResult = await mediator.Send(adminQuery);

        if (adminResult.IsFailed)
        {
            return adminResult.ToNotFoundProblem();
        }

        var query = new GetAdminNotificatioinsQuery()
        {
            AdminId = adminId
        };

        var notifications = await mediator.Send(query);

        return TypedResults.Ok(notifications);
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

    public async Task<Results<Ok<AdminDto>, NotFound<ProblemDetails>, ValidationProblem>> GetAdminById(
        Guid adminId, IValidator<GetAdminByIdQuery> validator, IMediator mediator
    )
    {
        var query = new GetAdminByIdQuery()
        {
            AdminId = adminId,
        };

        var validationResult = await validator.ValidateAsync(query);

        if (!validationResult.IsValid)
        {
            return validationResult.ToValidationProblem();
        }

        var result = await mediator.Send(query);

        if (result.IsFailed)
        {
            return result.ToNotFoundProblem();
        }

        return TypedResults.Ok(result.Value);
    }

    public async Task<Ok<IEnumerable<AdminDto>>> GetAllAdmins(IMediator mediator)
    {
        var query = new GetAllAdminsQuery();
        var admins = await mediator.Send(query);
        return TypedResults.Ok(admins);
    }

    public async Task<Results<Ok, BadRequest<ProblemDetails>, ValidationProblem>> LoginAdmin(
        LoginAdminCommand command, IValidator<LoginAdminCommand> validator, IMediator mediator
    )
    {
        var validationResult = await validator.ValidateAsync(command);

        if (!validationResult.IsValid)
        {
            return validationResult.ToValidationProblem();
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
        var validationResult = await validator.ValidateAsync(command);

        if (!validationResult.IsValid)
        {
            return validationResult.ToValidationProblem();
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