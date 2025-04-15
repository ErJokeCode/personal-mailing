using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Chatter.Data;
using Shared.Infrastructure.Rest;
using Chatter.Services.UserAccessor;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Chatter.Features.Chats;
using Chatter.Services;
using Shared.Infrastructure.Errors;
using Chatter.Features.Chats.DTOs;

namespace Chatter.Features.Chats.Queries;

public static class GetMessagesQuery
{
    public class Request
    {
        public required Guid StudentId { get; set; }

        public int? Page { get; set; }
        public int? PageSize { get; set; }
    }

    public static async Task<Results<Ok<PagedList<MessageDto>>, NotFound<ProblemDetails>>> Handle(
        [AsParameters] Request request,
        AppDbContext db,
        ChatMapper chatMapper,
        IUserAccessor userAccessor,
        ChatService chatService
    )
    {
        var assignedToChat = await chatService.CheckAssignedAsync(request.StudentId);

        if (assignedToChat.IsFailed)
        {
            return assignedToChat.ToNotFoundProblem();
        }

        var admin = await userAccessor.GetUserAsync();

        if (admin is null)
        {
            return TypedResults.Ok(PagedList<MessageDto>.Create([], request.Page, request.PageSize));
        }

        var chat = await db.Chats
            .Include(ch => ch.Messages)
            .ThenInclude(m => m.Admin)
            .AsSplitQuery()
            .SingleOrDefaultAsync(ch => ch.AdminId == admin.Id && ch.StudentId == request.StudentId);

        if (chat is null)
        {
            return TypedResults.Ok(PagedList<MessageDto>.Create([], request.Page, request.PageSize));
        }

        chat.Messages = chat.Messages.OrderByDescending(m => m.CreatedAt).ToList();

        // If we get this, it means we want to know the page, where we have an unread message
        // In a chat, we should start from an unread message, instead of at the very bottom
        if (request.Page == -1)
        {
            var firstUnread = chat.Messages.Index().FirstOrDefault(m => !m.Item.IsRead);
            request.Page = firstUnread.Index / (request.PageSize ?? 1);
        }

        var paged = PagedList<MessageDto>.Create(chatMapper.Map(chat.Messages), request.Page, request.PageSize);

        return TypedResults.Ok(paged);
    }
}