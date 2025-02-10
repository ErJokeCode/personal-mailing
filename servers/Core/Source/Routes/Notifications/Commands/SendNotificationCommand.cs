using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Data;
using Core.External.TelegramBot;
using Core.Infrastructure.Services;
using Core.Models;
using Core.Routes.Notifications.Dtos;
using Core.Routes.Notifications.Maps;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Routes.Notifications.Commands;

public class SendNotificationCommand : IRequest<Result<NotificationDto>>
{
    public required string Content { get; set; }
    public required IEnumerable<Guid> StudentIds { get; set; }
}

public class SendNotificationCommandHandler : IRequestHandler<SendNotificationCommand, Result<NotificationDto>>
{
    private readonly AppDbContext _db;
    private readonly IUserAccessor _userAccessor;
    private readonly ITelegramBot _telegramBot;
    private readonly NotificationMapper _notificationMapper;

    public SendNotificationCommandHandler(AppDbContext db, IUserAccessor userAccessor, ITelegramBot telegramBot)
    {
        _db = db;
        _userAccessor = userAccessor;
        _telegramBot = telegramBot;
        _notificationMapper = new NotificationMapper();
    }

    public async Task<Result<NotificationDto>> Handle(SendNotificationCommand request, CancellationToken cancellationToken)
    {
        var admin = await _userAccessor.GetUserAsync();

        if (admin is null)
        {
            return Result.Fail<NotificationDto>("Админ не был найден");
        }

        var notification = new Notification()
        {
            Content = request.Content,
            CreatedAt = DateTime.UtcNow,
            AdminId = admin.Id,
            Admin = admin,
        };

        foreach (var studentId in request.StudentIds)
        {
            var student = await _db.Students.SingleOrDefaultAsync(s => s.Id == studentId);

            if (student is null)
            {
                return Result.Fail<NotificationDto>($"Студент с айди {studentId} не был найден");
            }

            notification.Students.Add(student);
        }

        foreach (var student in notification.Students)
        {
            await _telegramBot.SendNotificationAsync(student.ChatId, notification.Content);
        }

        await _db.Notifications.AddAsync(notification);
        await _db.SaveChangesAsync();

        return Result.Ok(_notificationMapper.Map(notification));
    }
}
