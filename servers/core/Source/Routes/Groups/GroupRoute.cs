using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Infrastructure.Errors;
using Core.Routes.Admins.Queries;
using Core.Routes.Groups.Commands;
using Core.Routes.Groups.DTOs;
using Core.Routes.Groups.Queries;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Core.Routes.Groups;

public class GroupRoute : IRoute
{
    public void MapRoutes(WebApplication app)
    {
        var group = app.MapGroup("/core/groups");

        group.MapGet("/", GetAllGroups)
            .WithDescription("Получет все привязывания групп");

        group.MapPatch("/", AssignGroups)
            .WithDescription("Привязывает группы к админу");
    }

    private async Task<Ok<IEnumerable<GroupAssignmentDto>>> GetAllGroups([AsParameters] GetAllGroupsQuery query, IMediator mediator)
    {
        var groups = await mediator.Send(query);

        return TypedResults.Ok(groups);
    }

    public async Task<Results<NoContent, NotFound<ProblemDetails>, ValidationProblem>> AssignGroups(
        AssignGroupsCommand command, IValidator<AssignGroupsCommand> validator, IMediator mediator
    )
    {
        if (validator.TryValidate(command, out var validation))
        {
            return validation.ToValidationProblem();
        }

        var adminQuery = new GetAdminByIdQuery()
        {
            AdminId = command.AdminId
        };

        var adminResult = await mediator.Send(adminQuery);

        if (adminResult.IsFailed)
        {
            return adminResult.ToNotFoundProblem();
        }

        var result = await mediator.Send(command);

        if (result.IsFailed)
        {
            return result.ToNotFoundProblem();
        }

        return TypedResults.NoContent();
    }
}