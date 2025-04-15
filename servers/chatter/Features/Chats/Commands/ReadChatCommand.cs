using System;
using System.Threading.Tasks;
using Chatter.Signal;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Chatter.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Chatter.Services;
using Shared.Infrastructure.Errors;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using FluentValidation;
using System.Linq;

namespace Chatter.Features.Chats.Commands;

public static class ReadChatCommand
{
    public class Request
    {
        public required Guid StudentId { get; set; }
        public required IEnumerable<int> MessageIds { get; set; } = [];
    }

    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.MessageIds)
                .NotEmpty()
                .WithName("Сообщения");
        }
    }

    public static async Task<Results<NoContent, NotFound<ProblemDetails>, ValidationProblem>> Handle(
        Request request,
        IValidator<Request> validator,
        AppDbContext db,
        ChatService chatService,
        IHubContext<SignalHub> hub,
        ChatMapper chatMapper
    )
    {
        if (validator.TryValidate(request, out var validation))
        {
            return validation.ToValidationProblem();
        }

        var assignedToChat = await chatService.CheckAssignedAsync(request.StudentId);

        if (assignedToChat.IsFailed)
        {
            return assignedToChat.ToNotFoundProblem();
        }

        var chat = await db.Chats.SingleAsync(ch => ch.StudentId == request.StudentId);
        var messages = await db.Messages.Where(m => m.ChatId == chat.Id && request.MessageIds.Contains(m.Id)).ToListAsync();

        var readCount = 0;

        foreach (var message in messages)
        {
            message.IsRead = true;
            readCount += 1;
        }

        chat.UnreadCount -= readCount;

        await db.SaveChangesAsync();

        var dto = chatMapper.Map(chat);
        await hub.NotifyOfChatRead(chat.AdminId, dto);

        return TypedResults.NoContent();
    }
}