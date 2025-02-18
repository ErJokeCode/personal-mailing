using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Core.Identity;
using Core.Infrastructure.Errors;
using Core.Infrastructure.Metadata;
using Core.Routes.Chats.Commands;
using Core.Routes.Chats.DTOs;
using Core.Routes.Chats.Queries;
using Core.Routes.Students.Queries;
using FluentResults;
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
    }

    public async Task<Results<Ok<ChatDto>, NotFound<ProblemDetails>, ValidationProblem>> GetChatById(
        Guid studentId, IValidator<GetStudentByIdQuery> validator, IMediator mediator
    )
    {
        var studentQuery = new GetStudentByIdQuery()
        {
            StudentId = studentId
        };

        var studentValidation = await validator.ValidateAsync(studentQuery);

        if (!studentValidation.IsValid)
        {
            return studentValidation.ToValidationProblem();
        }

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
        [FromForm] IFormFileCollection documents, [FromForm] string body, IValidator<GetStudentByIdQuery> studentValidator, IValidator<SendMessageFromStudentCommand> validator, IMediator mediator
    )
    {
        var details = JsonSerializer.Deserialize<MessageDetails>(
            body,
            new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }
        );

        var query = new GetStudentByIdQuery()
        {
            StudentId = details?.StudentId ?? Guid.Empty
        };

        var studentValidation = await studentValidator.ValidateAsync(query);

        if (!studentValidation.IsValid)
        {
            studentValidation.ToValidationProblem();
        }

        var studentResult = await mediator.Send(query);

        if (studentResult.IsFailed)
        {
            return studentResult.ToBadRequestProblem();
        }

        var command = new SendMessageFromStudentCommand()
        {
            Content = details?.Content ?? "",
            StudentId = details?.StudentId ?? Guid.Empty,
            FormFiles = documents,
        };

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

    public async Task<Ok<IEnumerable<ChatDto>>> GetChats(IMediator mediator)
    {
        var query = new GetChatsQuery();

        var chats = await mediator.Send(query);

        return TypedResults.Ok(chats);
    }

    private class MessageDetails
    {
        public required string Content { get; set; }
        public required Guid StudentId { get; set; }
    }

    public async Task<Results<Ok<MessageDto>, BadRequest<ProblemDetails>, ValidationProblem>> SendMessage(
        [FromForm] IFormFileCollection documents, [FromForm] string body, IValidator<GetStudentByIdQuery> studentValidator, IValidator<SendMessageCommand> validator, IMediator mediator
    )
    {
        var details = JsonSerializer.Deserialize<MessageDetails>(
            body,
            new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }
        );

        var query = new GetStudentByIdQuery()
        {
            StudentId = details?.StudentId ?? Guid.Empty
        };

        var studentValidation = await studentValidator.ValidateAsync(query);

        if (!studentValidation.IsValid)
        {
            studentValidation.ToValidationProblem();
        }

        var studentResult = await mediator.Send(query);

        if (studentResult.IsFailed)
        {
            return studentResult.ToBadRequestProblem();
        }

        var command = new SendMessageCommand()
        {
            Content = details?.Content ?? "",
            StudentId = details?.StudentId ?? Guid.Empty,
            FormFiles = documents,
        };

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