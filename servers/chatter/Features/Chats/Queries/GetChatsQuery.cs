using System.Linq;
using System.Threading.Tasks;
using Chatter.Models;
using Microsoft.EntityFrameworkCore;
using Chatter.Data;
using Shared.Infrastructure.Rest;
using Chatter.Services.UserAccessor;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http;
using Chatter.Features.Chats.DTOs;

namespace Chatter.Features.Chats.Queries;

public static class GetChatsQuery
{
    public class Request
    {
        public string? Search { get; set; }
        public int? Page { get; set; }
        public int? PageSize { get; set; }

        public bool? OnlyUnread { get; set; } = false;
    }

    public static async Task<Ok<PagedList<ChatDto>>> Handle(
        [AsParameters] Request request,
        AppDbContext db,
        ChatMapper chatMapper,
        IUserAccessor userAccessor
    )
    {
        var admin = await userAccessor.GetUserAsync();

        if (admin is null)
        {
            return TypedResults.Ok(PagedList<ChatDto>.Create([], request.Page, request.PageSize));
        }

        var chatsQuery = db.Chats
            .Include(ch => ch.Student)
            .Include(ch => ch.Messages.OrderByDescending(m => m.CreatedAt).Take(1))
            .ThenInclude(m => m.Admin)
            .Where(ch => ch.AdminId == admin.Id)
            .AsSplitQuery();

        var chats = await FilterChats(chatsQuery, request).ToListAsync();

        chats = chats.OrderByDescending(ch => ch.Messages.Single().CreatedAt).ToList();

        var paged = PagedList<ChatDto>.Create(chatMapper.Map(chatsQuery), request.Page, request.PageSize);

        return TypedResults.Ok(paged);
    }

    private static IQueryable<Models.Chat> FilterChats(IQueryable<Models.Chat> chats, Request request)
    {
        if (!string.IsNullOrEmpty(request.Search))
        {
            var getFullName = (Student s) => $"{s.Info.Surname} {s.Info.Name} {s.Info.Patronymic ?? ""}";

            chats = chats
                .Where(ch =>
                    ch.Student!.Email.ToLower().Contains(request.Search.ToLower())
                    || getFullName(ch.Student).ToLower().Contains(request.Search.ToLower())
                );
        }

        if (request.OnlyUnread is not null && request.OnlyUnread is true)
        {
            chats = chats.Where(ch => ch.UnreadCount > 0);
        }

        return chats;
    }
}