using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Core.Identity;
using Core.Infrastructure.Errors;
using Core.Routes.Chats.Commands;
using Core.Routes.Chats.DTOs;
using Core.Routes.Chats.Maps;
using Core.Routes.Chats.Queries;
using Core.Routes.Students.Queries;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Core.Routes.Chats;

public class ChatRoute : IRoute
{
    public void MapRoutes(WebApplication app)
    {
        var group = app.MapGroup("/core/chats")
            .RequireAuthorization();

        group.MapGet("/", GetChats)
            .WithDescription("Получает чаты админа");

        group.MapPost("/", SendMessage)
            .WithDescription("Отправляет сообщение студенту")
            .DisableAntiforgery();

        group.MapPost("/from-student", SendMessageFromStudent)
            .WithDescription("Отправляет сообщение админу")
            .WithTags(SecretTokenAuthentication.Tag)
            .RequireAuthorization(SecretTokenAuthentication.Policy)
            .DisableAntiforgery();

        group.MapGet("/{studentId}", GetChatById)
            .WithDescription("Получает чат со студентом по айди");

        group.MapPatch("/{studentId}/read", ReadChat)
            .WithDescription("Делает чат прочитанным");
    }

    private async Task<Results<NoContent, NotFound<ProblemDetails>, ValidationProblem>> ReadChat(
        Guid studentId, IMediator mediator
    )
    {
        var studentQuery = new GetStudentByIdQuery()
        {
            StudentId = studentId
        };

        var studentResult = await mediator.Send(studentQuery);

        if (studentResult.IsFailed)
        {
            return studentResult.ToNotFoundProblem();
        }

        var command = new ReadChatCommand()
        {
            StudentId = studentId,
        };

        await mediator.Send(command);

        return TypedResults.NoContent();
    }

    public async Task<Results<Ok<ChatDto>, NotFound<ProblemDetails>>> GetChatById(
        Guid studentId, IMediator mediator
    )
    {
        var studentQuery = new GetStudentByIdQuery()
        {
            StudentId = studentId
        };

        var studentResult = await mediator.Send(studentQuery);

        if (studentResult.IsFailed)
        {
            return studentResult.ToNotFoundProblem();
        }

        var query = new GetChatById()
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

    public async Task<Results<Ok<MessageDto>, BadRequest<ProblemDetails>, ValidationProblem>> SendMessageFromStudent(
        [FromForm] IFormFileCollection documents, [FromForm] string body, IValidator<SendMessageFromStudentCommand> validator, IMediator mediator
    )
    {
        var sendDto = new ChatMapper().Map(body);

        var query = new GetStudentByIdQuery()
        {
            StudentId = sendDto?.StudentId ?? Guid.Empty
        };

        var studentResult = await mediator.Send(query);

        if (studentResult.IsFailed)
        {
            return studentResult.ToBadRequestProblem();
        }

        var command = new SendMessageFromStudentCommand()
        {
            Content = sendDto?.Content ?? "",
            StudentId = sendDto?.StudentId ?? Guid.Empty,
            FormFiles = documents,
        };

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

    public async Task<Ok<IEnumerable<ChatDto>>> GetChats([AsParameters] GetChatsQuery query, IMediator mediator)
    {
        var chats = await mediator.Send(query);

        return TypedResults.Ok(chats);
    }

    public async Task<Results<Ok<MessageDto>, BadRequest<ProblemDetails>, ValidationProblem>> SendMessage(
        [FromForm] IFormFileCollection documents, [FromForm] string body, IValidator<SendMessageCommand> validator, IMediator mediator
    )
    {
        var sendDto = new ChatMapper().Map(body);

        var query = new GetStudentByIdQuery()
        {
            StudentId = sendDto?.StudentId ?? Guid.Empty
        };

        var studentResult = await mediator.Send(query);

        if (studentResult.IsFailed)
        {
            return studentResult.ToBadRequestProblem();
        }

        var command = new SendMessageCommand()
        {
            Content = sendDto?.Content ?? "",
            StudentId = sendDto?.StudentId ?? Guid.Empty,
            FormFiles = documents,
        };

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