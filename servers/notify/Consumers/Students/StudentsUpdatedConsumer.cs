using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Notify.Abstractions.Parser;
using Notify.Data;
using Notify.Models;
using Notify.Routes.Notifications.Maps;
using Shared.Messages.Students;

namespace Notify.Consumers.Students;

class StudentsUpdatedConsumer : IConsumer<StudentsUpdatedMessage>
{
    private readonly AppDbContext _db;
    private readonly NotificationMapper _notificationMapper;

    public StudentsUpdatedConsumer(AppDbContext db)
    {
        _db = db;
        _notificationMapper = new NotificationMapper();
    }

    public async Task Consume(ConsumeContext<StudentsUpdatedMessage> context)
    {
        foreach (var studentDto in context.Message.Students)
        {
            var student = await _db.Students.FindAsync(studentDto.Id);

            if (student is null)
            {
                var newStudent = _notificationMapper.Map(studentDto);

                await _db.Students.AddAsync(newStudent);
            }
            else
            {
                student = _notificationMapper.Map(studentDto);
            }
        }

        await _db.SaveChangesAsync();
    }
}
