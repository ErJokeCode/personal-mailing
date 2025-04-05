using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Routes.Chats.Maps;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Notify.Data;

namespace Core.Routes.Chats.Commands;

public class ReadChatCommand : IRequest<Unit>
{
    public required Guid StudentId { get; set; }
}

public class ReadChatCommandHandler : IRequestHandler<ReadChatCommand, Unit>
{
    private readonly AppDbContext _db;
    // private readonly IHubContext<SignalHub> _hub;
    private readonly ChatMapper _chatMapper;

    // public ReadChatCommandHandler(AppDbContext db, IHubContext<SignalHub> hub)
    // {
    //     _db = db;
    //     _hub = hub;
    //     _chatMapper = new ChatMapper();
    // }

    public ReadChatCommandHandler(AppDbContext db)
    {
        _db = db;
        _chatMapper = new ChatMapper();
    }

    public async Task<Unit> Handle(ReadChatCommand request, CancellationToken cancellationToken)
    {
        var chat = await _db.Chats.SingleAsync(ch => ch.StudentId == request.StudentId);

        foreach (var message in chat.Messages)
        {
            message.IsRead = true;
        }

        chat.UnreadCount = 0;

        await _db.SaveChangesAsync();

        var dto = _chatMapper.Map(chat);
        // await _hub.NotifyOfChatRead(chat.AdminId, dto);

        return Unit.Value;
    }
}
