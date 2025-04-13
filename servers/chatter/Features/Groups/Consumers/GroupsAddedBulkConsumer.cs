using System.Threading.Tasks;
using Chatter.Routes.Chats.Maps;
using MassTransit;
using Chatter.Data;
using Shared.Context.Groups.Messages;

namespace Chatter.Features.Groups.Consumers;

class GroupsAddedBulkConsumer : IConsumer<GroupsAddedBulkMessage>
{
    private readonly AppDbContext _db;
    private readonly GroupMapper _groupMapper;

    public GroupsAddedBulkConsumer(AppDbContext db, GroupMapper groupMapper)
    {
        _db = db;
        _groupMapper = groupMapper;
    }

    public async Task Consume(ConsumeContext<GroupsAddedBulkMessage> context)
    {
        var message = context.Message;

        foreach (var groupAssignment in message.Groups)
        {
            var groupAssignmentReal = _groupMapper.Map(groupAssignment);
            var admin = await _db.Users.FindAsync(groupAssignment.AdminId);

            groupAssignmentReal.Admin = admin;

            await _db.GroupAssignments.AddAsync(groupAssignmentReal);
        }

        await _db.SaveChangesAsync();
    }
}
