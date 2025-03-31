using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Data;
using Core.Routes.Chats.Maps;
using Core.Signal;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Core.Routes.Groups.Commands;

public class AssignGroupsCommand : IRequest<Result>
{
    public required Guid AdminId { get; set; }
    public IEnumerable<int> GroupIds { get; set; } = [];
}

public class AssignGroupsCommandHandler : IRequestHandler<AssignGroupsCommand, Result>
{
    private readonly AppDbContext _db;
    private readonly IHubContext<SignalHub> _hub;
    private readonly ChatMapper _chatMapper;

    public AssignGroupsCommandHandler(AppDbContext db, IHubContext<SignalHub> hub)
    {
        _db = db;
        _hub = hub;
        _chatMapper = new ChatMapper();
    }

    public async Task<Result> Handle(AssignGroupsCommand request, CancellationToken cancellationToken)
    {
        var groups = await _db.GroupAssignments
            .Where(g => request.GroupIds.Contains(g.Id))
            .ToListAsync();

        foreach (var group in groups)
        {
            var oldAdmin = await _db.Users
                .Include(a => a.Chats)
                .ThenInclude(ch => ch.Student)
                .SingleAsync(a => a.Id == group.AdminId);

            var chats = oldAdmin.Chats.Where(ch => ch.Student!.Info.Group.Number == group.Name);

            foreach (var chat in chats)
            {
                chat.AdminId = request.AdminId;

                if (chat.UnreadCount > 0)
                {
                    await _hub.NotifyOfChatRead(request.AdminId, _chatMapper.Map(chat));
                }
            }

            group.AdminId = request.AdminId;
        }

        await _db.SaveChangesAsync();

        return Result.Ok();
    }
}