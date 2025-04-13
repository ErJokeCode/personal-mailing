using System.Threading.Tasks;
using Chatter.Routes.Chats.Maps;
using MassTransit;
using Chatter.Data;
using Shared.Context.Admins.Messages;

namespace Chatter.Features.Admins.Consumers;

class AdminCreatedConsumer : IConsumer<AdminCreatedMessage>
{
    private readonly AppDbContext _db;
    private readonly AdminMapper _adminMapper;

    public AdminCreatedConsumer(AppDbContext db, AdminMapper adminMapper)
    {
        _db = db;
        _adminMapper = adminMapper;
    }

    public async Task Consume(ConsumeContext<AdminCreatedMessage> context)
    {
        var exists = await _db.Users.FindAsync(context.Message.Id);

        if (exists != null)
        {
            return;
        }

        var admin = _adminMapper.Map(context.Message);

        await _db.Users.AddAsync(admin);
        await _db.SaveChangesAsync();
    }
}
