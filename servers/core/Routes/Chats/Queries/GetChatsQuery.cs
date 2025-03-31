using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Abstractions.UserAccesor;
using Core.Data;
using Core.Infrastructure.Rest;
using Core.Models;
using Core.Routes.Admins.Errors;
using Core.Routes.Chats.DTOs;
using Core.Routes.Chats.Maps;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Routes.Chats.Queries;

public class GetChatsQuery : IRequest<PagedList<ChatDto>>
{
    public string? Search { get; set; }
    public int? Page { get; set; }
    public int? PageSize { get; set; }

    public bool? OnlyUnread { get; set; } = false;
}

public class GetChatsQueryHandler : IRequestHandler<GetChatsQuery, PagedList<ChatDto>>
{
    private readonly AppDbContext _db;
    private readonly ChatMapper _chatMapper;
    private readonly IUserAccessor _userAccessor;

    public GetChatsQueryHandler(AppDbContext db, IUserAccessor userAccessor)
    {
        _db = db;
        _chatMapper = new ChatMapper();
        _userAccessor = userAccessor;
    }

    public async Task<PagedList<ChatDto>> Handle(GetChatsQuery request, CancellationToken cancellationToken)
    {
        var admin = await _userAccessor.GetUserAsync();

        if (admin is null)
        {
            return PagedList<ChatDto>.Create([], request.Page, request.PageSize);
        }

        var chats = await _db.Chats
            .Include(ch => ch.Student)
            .Include(ch => ch.Messages.OrderByDescending(m => m.CreatedAt).Take(1))
            .ThenInclude(m => m.Admin)
            .Where(ch => ch.AdminId == admin.Id)
            .AsSplitQuery()
            .ToListAsync();

        chats = FilterChats(chats, request).ToList();

        chats = chats.OrderByDescending(ch => ch.Messages.Single().CreatedAt).ToList();

        return PagedList<ChatDto>.Create(_chatMapper.Map(chats), request.Page, request.PageSize);
    }

    private IEnumerable<Chat> FilterChats(IEnumerable<Chat> chats, GetChatsQuery request)
    {
        if (!string.IsNullOrEmpty(request.Search))
        {
            var getFullName = (Student s) => $"{s.Info.Surname} {s.Info.Name} {s.Info.Patronymic ?? ""}";

            chats = chats
                .Where(ch =>
                    FuzzySearch.Contains(ch.Student!.Email, request.Search)
                    || FuzzySearch.Contains(getFullName(ch.Student), request.Search)
                );
        }

        if (request.OnlyUnread is not null && request.OnlyUnread is true)
        {
            chats = chats.Where(ch => ch.UnreadCount > 0);
        }

        return chats;
    }
}
