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

namespace Chatter.Features.Chats.Commands;

public static class ReadChatCommand
{
    public static async Task<Results<NoContent, NotFound<ProblemDetails>, ValidationProblem>> Handle(
        Guid studentId,
        AppDbContext db,
        ChatService chatService,
        IHubContext<SignalHub> hub,
        ChatMapper chatMapper
    )
    {
        var assignedToChat = await chatService.CheckAssignedAsync(studentId);

        if (assignedToChat.IsFailed)
        {
            return assignedToChat.ToNotFoundProblem();
        }

        var chat = await db.Chats.SingleAsync(ch => ch.StudentId == studentId);

        foreach (var message in chat.Messages)
        {
            message.IsRead = true;
        }

        chat.UnreadCount = 0;

        await db.SaveChangesAsync();

        var dto = chatMapper.Map(chat);
        await hub.NotifyOfChatRead(chat.AdminId, dto);

        return TypedResults.NoContent();
    }
}