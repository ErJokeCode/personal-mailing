using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Infrastructure.Errors;
using Core.Routes.Admins.Queries;
using Core.Routes.Students.Commands;
using Core.Routes.Students.Dtos;
using Core.Routes.Students.Queries;
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
        var group = app.MapGroup("/students")
            .RequireAuthorization();

        group.MapPost("/auth", AuthStudent)
            .WithDescription("Auths the student");

        group.MapGet("/", GetAllStudents)
            .WithDescription("Gets all students");

        group.MapGet("/{studentId}", GetStudentById)
            .WithDescription("Gets a student by id");
    }

    private async Task<Results<Ok<StudentDto>, NotFound<ProblemDetails>, ValidationProblem>> GetStudentById(
        Guid studentId, IValidator<GetStudentByIdQuery> validator, IMediator mediator
    )
    {
        var query = new GetStudentByIdQuery()
        {
            StudentId = studentId,
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

    public async Task<Ok<IEnumerable<StudentDto>>> GetAllStudents(IMediator mediator)
    {
        var query = new GetAllStudentsQuery();

        var result = await mediator.Send(query);

        return TypedResults.Ok(result);
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