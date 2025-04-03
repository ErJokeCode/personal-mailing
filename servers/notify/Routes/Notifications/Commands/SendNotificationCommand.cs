using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Notify.Abstractions.MailService;
using Notify.Abstractions.UserAccessor;
using Notify.Data;
using Notify.Models;
using Notify.Routes.Notifications.DTOs;
using Notify.Routes.Notifications.Errors;
using Notify.Routes.Notifications.Maps;
using Shared.Abstractions.FileStorage;

namespace Notify.Routes.Notifications.Commands;

public class SendNotificationCommand : IRequest<Result<NotificationDto>>
{
    public required string Content { get; set; }
    public required IEnumerable<Guid> StudentIds { get; set; }
    public IFormFileCollection? FormFiles { get; set; }
}

public class SendNotificationCommandHandler : IRequestHandler<SendNotificationCommand, Result<NotificationDto>>
{
    private readonly AppDbContext _db;
    private readonly IUserAccessor _userAccessor;
    private readonly IMailService _mailService;
    private readonly NotificationMapper _notificationMapper;
    private readonly IFileStorage _fileStorage;

    public SendNotificationCommandHandler(AppDbContext db, IUserAccessor userAccessor, IMailService mailService, IFileStorage fileStorage)
    {
        _db = db;
        _userAccessor = userAccessor;
        _mailService = mailService;
        _notificationMapper = new NotificationMapper();
        _fileStorage = fileStorage;
    }

    public async Task<Result<NotificationDto>> Handle(SendNotificationCommand request, CancellationToken cancellationToken)
    {
        var adminInfo = await _userAccessor.GetUserAsync();

        if (adminInfo is null)
        {
            return Result.Fail<NotificationDto>(AdminErrors.NotFound());
        }

        var admin = await _db.Users.FindAsync(adminInfo.Id);

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

        if (request.FormFiles is not null && request.FormFiles.Count > 0)
        {
            foreach (var formFile in request.FormFiles)
            {
                var blobData = new BlobData()
                {
                    Stream = formFile.OpenReadStream(),
                    ContentType = formFile.ContentType,
                    Name = formFile.FileName,
                };

                var document = await _fileStorage.UploadAsync(blobData);

                notification.Documents.Add(document);
            }
        }

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
            var sent = await _mailService.SendNotificationAsync(student.ChatId, notification.Content, notification.Documents);

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
