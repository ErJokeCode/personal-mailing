using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Abstractions.UserAccesor;
using Core.Data;
using Core.Models;
using Core.Routes.Admins.Errors;
using Core.Routes.Chats.DTOs;
using Core.Routes.Chats.Maps;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Routes.Chats.Queries;

// TODO add a general Search parameter everywhere, that's gonna look for most important data

public class GetChatsQuery : IRequest<IEnumerable<ChatDto>>;

public class GetChatsQueryHandler : IRequestHandler<GetChatsQuery, IEnumerable<ChatDto>>
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

    public async Task<IEnumerable<ChatDto>> Handle(GetChatsQuery request, CancellationToken cancellationToken)
    {
        var admin = await _userAccessor.GetUserAsync();

        if (admin is null)
        {
            return [];
        }

        var chats = await _db.Chats
            .Include(ch => ch.Student)
            .Where(ch => ch.AdminId == admin.Id)
            .AsSplitQuery()
            .ToListAsync();

        foreach (var chat in chats)
        {
            chat.Messages = chat.Messages.OrderByDescending(m => m.CreatedAt).Take(1).ToList();
        }

        return _chatMapper.Map(chats);
    }
}
