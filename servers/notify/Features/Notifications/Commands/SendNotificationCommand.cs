using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentResults;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Notify.Data;
using Notify.Features.Notifications.DTOs;
using Notify.Models;
using Notify.Routes.Notifications.Maps;
using Notify.Services.MailService;
using Notify.Services.UserAccessor;
using Shared.Context.Admins;
using Shared.Context.Notifications;
using Shared.Context.Students;
using Shared.Infrastructure.Errors;
using Shared.Services.FileStorage;

namespace Notify.Features.Notifications.Commands;

public static class SendNotificationCommand
{
    public class Request
    {
        public required string Content { get; set; }
        public required IEnumerable<Guid> StudentIds { get; set; }
        public IFormFileCollection? FormFiles { get; set; }
    }

    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.Content)
                .NotEmpty()
                .When(x => !x.FormFiles?.Any() ?? true)
                .WithName("Содержание");

            RuleFor(x => x.StudentIds)
                .NotEmpty()
                .WithName("Студенты");
        }
    }

    public static async Task<Results<Ok<NotificationDto>, BadRequest<ProblemDetails>, ValidationProblem>> Handle(
        [FromForm] IFormFileCollection documents,
        [FromForm] string body,
        IValidator<Request> validator,
        NotificationMapper notificationMapper,
        IUserAccessor userAccessor,
        AppDbContext db,
        IFileStorage fileStorage,
        IMailService mailService
    )
    {
        var sendNotification = notificationMapper.Map(body);

        var request = new Request()
        {
            Content = sendNotification?.Content ?? "",
            StudentIds = sendNotification?.StudentIds ?? [],
            FormFiles = documents,
        };

        if (validator.TryValidate(request, out var validation))
        {
            return validation.ToValidationProblem();
        }

        var admin = await userAccessor.GetUserAsync();

        if (admin is null)
        {
            return Result.Fail<NotificationDto>(AdminErrors.NotFound()).ToBadRequestProblem();
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

                var document = await fileStorage.UploadAsync(blobData);

                notification.Documents.Add(document);
            }
        }

        foreach (var studentId in request.StudentIds)
        {
            var student = await db.Students.SingleOrDefaultAsync(s => s.Id == studentId);

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
            var sent = await mailService.SendNotificationAsync(student.ChatId, notification.Content, notification.Documents);

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

        await db.Notifications.AddAsync(notification);
        await db.SaveChangesAsync();

        return TypedResults.Ok(notificationMapper.Map(notification));
    }
}