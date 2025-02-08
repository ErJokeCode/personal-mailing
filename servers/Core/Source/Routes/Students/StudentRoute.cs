using System;
using System.Threading.Tasks;
using Core.Infrastructure.Errors;
using Core.Routes.Students.Commands;
using Core.Routes.Students.Dtos;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Core.Routes.Students;

public class StudentRoute : IRoute
{
    public void MapRoutes(WebApplication app)
    {
        var group = app.MapGroup("/student").RequireAuthorization();

        group.MapPost("/auth", AuthStudent);
    }

    public async Task<Results<Ok<StudentDto>, BadRequest<ProblemDetails>, ValidationProblem>> AuthStudent(
        AuthStudentCommand command, IValidator<AuthStudentCommand> validator, IMediator mediator
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
}