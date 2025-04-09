using System.Threading.Tasks;
using Chatter.Routes.Chats.Maps;
using MassTransit;
using Chatter.Data;
using Shared.Messages.Admins;

namespace Chatter.Consumers.Admins;

class AdminCreatedConsumer : IConsumer<AdminCreatedMessage>
{
    private readonly AppDbContext _db;
    private readonly ChatMapper _chatMapper;

    public AdminCreatedConsumer(AppDbContext db)
    {
        _db = db;
        _chatMapper = new ChatMapper();
    }

    public async Task Consume(ConsumeContext<AdminCreatedMessage> context)
    {
        var exists = await _db.Users.FindAsync(context.Message.Admin.Id);

        if (exists != null)
        {
            return;
        }

        var adminDto = context.Message.Admin;
        var admin = _chatMapper.Map(adminDto);

        await _db.Users.AddAsync(admin);
        await _db.SaveChangesAsync();
    }
}
