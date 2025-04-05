using System.Threading.Tasks;
using Core.Routes.Chats.Maps;
using MassTransit;
using Notify.Data;
using Shared.Messages.Admins;
using Shared.Messages.Groups;

namespace Notify.Consumers.Groups;

class GroupsAddedConsumer : IConsumer<GroupsAddedMessage>
{
    private readonly AppDbContext _db;
    private readonly ChatMapper _chatMapper;

    public GroupsAddedConsumer(AppDbContext db)
    {
        _db = db;
        _chatMapper = new ChatMapper();
    }

    public async Task Consume(ConsumeContext<GroupsAddedMessage> context)
    {
        var message = context.Message;

        foreach (var groupAssignment in message.GroupAssignments)
        {
            var groupAssignmentReal = _chatMapper.Map(groupAssignment);
            var admin = await _db.Users.FindAsync(groupAssignment.AdminId);

            groupAssignmentReal.Admin = admin;

            await _db.GroupAssignments.AddAsync(groupAssignmentReal);
        }

        await _db.SaveChangesAsync();
    }
}
