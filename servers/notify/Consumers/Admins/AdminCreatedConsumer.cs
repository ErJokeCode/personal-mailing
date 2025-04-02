using System;
using System.Reactive;
using System.Threading.Tasks;
using MassTransit;
using Notify.Data;
using Notify.Messages.Admins;
using Notify.Models;
using Notify.Routes.Notifications.Maps;

namespace Notify.Consumers.Admins;

class AdminCreatedConsumer : IConsumer<AdminCreatedMessage>
{
    private readonly AppDbContext _db;
    private readonly NotificationMapper _notificationMapper;

    public AdminCreatedConsumer(AppDbContext db)
    {
        _db = db;
        _notificationMapper = new NotificationMapper();
    }

    public async Task Consume(ConsumeContext<AdminCreatedMessage> context)
    {
        var exists = await _db.Users.FindAsync(context.Message.Admin.Id);

        if (exists != null)
        {
            return;
        }

        var adminDto = context.Message.Admin;
        var admin = _notificationMapper.Map(adminDto);

        await _db.Users.AddAsync(admin);
        await _db.SaveChangesAsync();
    }
}
