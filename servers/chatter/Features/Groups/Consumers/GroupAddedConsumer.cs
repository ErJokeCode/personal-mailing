using System.Threading.Tasks;
using MassTransit;
using Chatter.Data;
using Shared.Context.Groups.Messages;

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
        var admin = await _db.Users.FindAsync(message.AdminId);

        groupAssignmentReal.Admin = admin;

        await _db.GroupAssignments.AddAsync(groupAssignmentReal);
        await _db.SaveChangesAsync();
    }
}
