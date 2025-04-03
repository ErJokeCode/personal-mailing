using System;
using System.Threading.Tasks;
using MassTransit;
using Notify.Abstractions.Parser;
using Notify.Data;
using Notify.Models;
using Notify.Routes.Notifications.Maps;
using Shared.Messages.Students;

namespace Notify.Consumers.Students;


class StudentAuthedConsumer : IConsumer<StudentAuthedMessage>
{
    private readonly AppDbContext _db;
    private readonly NotificationMapper _notificationMapper;

    public StudentAuthedConsumer(AppDbContext db)
    {
        _db = db;
        _notificationMapper = new NotificationMapper();
    }

    public async Task Consume(ConsumeContext<StudentAuthedMessage> context)
    {
        var exists = await _db.Students.FindAsync(context.Message.Student.Id);

        if (exists != null)
        {
            return;
        }

        var studentDto = context.Message.Student;
        var student = _notificationMapper.Map(studentDto);

        await _db.Students.AddAsync(student);
        await _db.SaveChangesAsync();
    }
}