using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Identity;
using Core.Infrastructure.Errors;
using Core.Infrastructure.Metadata;
using Core.Infrastructure.Rest;
using Core.Routes.Admins.Queries;
using Core.Routes.Notifications.Dtos;
using Core.Routes.Students.Commands;
using Core.Routes.Students.Dtos;
using Core.Routes.Students.Queries;
using FluentValidation;
using FluentValidation.TestHelper;
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
        var group = app.MapGroup("/core/students")
            .RequireAuthorization();

        group.MapGet("/", GetAllStudents)
            .WithDescription("Получает всех студентов");

        group.MapPost("/auth", AuthStudent)
            .WithDescription("Аутентифицирует студента")
            .WithTags(SecretTokenAuthentication.Tag)
            .RequireAuthorization(SecretTokenAuthentication.Policy);

        group.MapGet("/{studentId}", GetStudentById)
            .WithDescription("Получает студента по айди");
    }

    private async Task<Results<Ok<StudentDto>, NotFound<ProblemDetails>>> GetStudentById(
        Guid studentId, IMediator mediator
    )
    {
        var query = new GetStudentByIdQuery()
        {
            StudentId = studentId,
        };

        var result = await mediator.Send(query);

        if (result.IsFailed)
        {
            return result.ToNotFoundProblem();
        }

        return TypedResults.Ok(result.Value);
    }

    public async Task<Ok<PagedList<StudentDto>>> GetAllStudents([AsParameters] GetAllStudentsQuery query, IMediator mediator)
    {
        var result = await mediator.Send(query);

        return TypedResults.Ok(result);
    }

    public async Task<Results<Ok<StudentDto>, BadRequest<ProblemDetails>, ValidationProblem>> AuthStudent(
        AuthStudentCommand command, IValidator<AuthStudentCommand> validator, IMediator mediator
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
}