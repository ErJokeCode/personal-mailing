using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Abstractions;
using Core.Data;
using Core.Infrastructure.Services;
using Core.Models;
using Core.Routes.Admins.Errors;
using Core.Routes.Notifications.Dtos;
using Core.Routes.Notifications.Errors;
using Core.Routes.Notifications.Maps;
using Core.Routes.Students.Errors;
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
    private readonly IMailService _mailService;
    private readonly NotificationMapper _notificationMapper;

    public SendNotificationCommandHandler(AppDbContext db, IUserAccessor userAccessor, IMailService mailService)
    {
        _db = db;
        _userAccessor = userAccessor;
        _mailService = mailService;
        _notificationMapper = new NotificationMapper();
    }

    public async Task<Result<NotificationDto>> Handle(SendNotificationCommand request, CancellationToken cancellationToken)
    {
        var admin = await _userAccessor.GetUserAsync();

        if (admin is null)
        {
            return Result.Fail<NotificationDto>(AdminErrors.NotFound());
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

            if (student is null || !student.Active)
            {
                var error = new NotificationError()
                {
                    StudentId = studentId,
                    Message = StudentErrors.NotFound(studentId),
                };
                notification.Errors.Add(error);
            }
            else
            {
                notification.Students.Add(student);
            }
        }

        foreach (var student in notification.Students)
        {
            var sent = await _mailService.SendNotificationAsync(student.ChatId, notification.Content);

            if (!sent)
            {
                var error = new NotificationError()
                {
                    StudentId = student.Id,
                    Message = NotificationErrors.CouldNotSend(student.Email),
                };
                notification.Errors.Add(error);
            }
        }

        await _db.Notifications.AddAsync(notification);
        await _db.SaveChangesAsync();

        return Result.Ok(_notificationMapper.Map(notification));
    }
}
