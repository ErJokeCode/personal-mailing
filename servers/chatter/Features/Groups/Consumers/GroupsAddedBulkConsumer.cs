using System.Threading.Tasks;
using MassTransit;
using Chatter.Data;
using Shared.Context.Groups.Messages;
using Shared.Context.Admins;

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

            var exists = await _db.GroupAssignments.FindAsync(groupAssignmentReal.Id);

            if (exists is not null)
            {
                continue;
            }

            var admin = await _db.Users.FindAsync(groupAssignment.AdminId);

            if (admin is null)
            {
                throw new System.Exception(AdminErrors.NotFound(groupAssignment.AdminId));
            }

            groupAssignmentReal.Admin = admin;

            await _db.GroupAssignments.AddAsync(groupAssignmentReal);
        }

        await _db.SaveChangesAsync();
    }
}
