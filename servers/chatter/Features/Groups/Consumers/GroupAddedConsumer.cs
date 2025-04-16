using System.Threading.Tasks;
using MassTransit;
using Chatter.Data;
using Shared.Context.Groups.Messages;
using Shared.Context.Admins;

namespace Chatter.Features.Groups.Consumers;

class GroupAddedConsumer : IConsumer<GroupAddedMessage>
{
    private readonly AppDbContext _db;
    private readonly GroupMapper _groupMapper;

    public GroupAddedConsumer(AppDbContext db, GroupMapper groupMapper)
    {
        _db = db;
        _groupMapper = groupMapper;
    }

    public async Task Consume(ConsumeContext<GroupAddedMessage> context)
    {
        var message = context.Message;

        var groupAssignmentReal = _groupMapper.Map(message);

        var exists = await _db.GroupAssignments.FindAsync(groupAssignmentReal.Id);

        if (exists is not null)
        {
            return;
        }

        var admin = await _db.Users.FindAsync(message.AdminId);

        if (admin is null)
        {
            throw new System.Exception(AdminErrors.NotFound(message.AdminId));
        }

        groupAssignmentReal.Admin = admin;

        await _db.GroupAssignments.AddAsync(groupAssignmentReal);
        await _db.SaveChangesAsync();
    }
}
