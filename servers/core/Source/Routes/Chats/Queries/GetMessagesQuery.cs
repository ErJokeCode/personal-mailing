using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Abstractions.UserAccesor;
using Core.Data;
using Core.Infrastructure.Rest;
using Core.Routes.Chats.DTOs;
using Core.Routes.Chats.Maps;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Routes.Chats.Queries;

public class GetMessagesQuery : IRequest<PagedList<MessageDto>>
{
    public required Guid StudentId { get; set; }

    public int? Page { get; set; }
    public int? PageSize { get; set; }
}

public class GetMessagesQueryHandler : IRequestHandler<GetMessagesQuery, PagedList<MessageDto>>
{
    private readonly AppDbContext _db;
    private readonly ChatMapper _chatMapper;
    private readonly IUserAccessor _userAccessor;

    public GetMessagesQueryHandler(AppDbContext db, IUserAccessor userAccessor)
    {
        _db = db;
        _chatMapper = new ChatMapper();
        _userAccessor = userAccessor;
    }

    public async Task<PagedList<MessageDto>> Handle(GetMessagesQuery request, CancellationToken cancellationToken)
    {
        var admin = await _userAccessor.GetUserAsync();

        if (admin is null)
        {
            return PagedList<MessageDto>.Create([], request.Page, request.PageSize);
        }

        var chat = await _db.Chats
            .Include(ch => ch.Messages)
            .ThenInclude(m => m.Admin)
            .AsSplitQuery()
            .SingleAsync(ch => ch.AdminId == admin.Id && ch.StudentId == request.StudentId);

        chat.Messages = chat.Messages.OrderByDescending(m => m.CreatedAt).ToList();

        // TODO add optimisations into all IQueryable statements, we can work with db directly, instead of ToList
        return PagedList<MessageDto>.Create(_chatMapper.Map(chat.Messages), request.Page, request.PageSize);
    }
}