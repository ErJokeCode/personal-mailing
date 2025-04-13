using System;
using System.Threading.Tasks;
using Chatter.Models;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Chatter.Data;
using Shared.Services.FileStorage;
using Shared.Context.Admins;
using Shared.Context.Chats;
using Shared.Context.Groups;
using Chatter.Services.MailService;
using Chatter.Services.UserAccessor;
using Chatter.Features.Chats.DTOs;
using FluentValidation;
using System.Linq;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Shared.Context.Students;
using Shared.Infrastructure.Errors;

namespace Chatter.Features.Chats.Commands;

public static class SendMessageCommand
{
    public class Request
    {
        public required Guid StudentId { get; set; }
        public required string Content { get; set; }
        public required IFormFileCollection FormFiles { get; set; }
    }

    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.Content)
                .NotEmpty()
                .When(x => !x.FormFiles?.Any() ?? true)
                .WithName("Содержание");
        }
    }

    public static async Task<Results<Ok<MessageDto>, BadRequest<ProblemDetails>, ValidationProblem>> Handle(
        [FromForm] IFormFileCollection documents,
        [FromForm] string body,
        IValidator<Request> validator,
        AppDbContext db,
        ChatMapper chatMapper,
        IMailService mailService,
        IFileStorage fileStorage,
        IUserAccessor userAccessor
    )
    {
        var sendMessage = chatMapper.Map(body);

        var request = new Request()
        {
            Content = sendMessage?.Content ?? "",
            StudentId = sendMessage?.StudentId ?? Guid.Empty,
            FormFiles = documents,
        };

        if (validator.TryValidate(request, out var validation))
        {
            return validation.ToValidationProblem();
        }

        var admin = await userAccessor.GetUserAsync();

        if (admin is null)
        {
            return Result.Fail<MessageDto>(AdminErrors.NotFound()).ToBadRequestProblem();
        }

        var student = await db.Students.SingleOrDefaultAsync(s => s.Id == request.StudentId);

        if (student is null)
        {
            return Result.Fail<MessageDto>(StudentErrors.NotFound(request.StudentId)).ToBadRequestProblem();
        }

        var groupAssignment = await db.GroupAssignments.SingleOrDefaultAsync(g => g.AdminId == admin.Id && g.Name == student.Info.Group.Number);

        if (groupAssignment is null || groupAssignment.Name != student.Info.Group.Number)
        {
            return Result.Fail<MessageDto>(GroupErrors.NotAssignedToGroup(admin.Email!, student.Info.Group.Number)).ToBadRequestProblem();
        }

        var chat = await db.Chats
            .AsSplitQuery()
            .SingleOrDefaultAsync(ch => ch.AdminId == admin.Id && ch.StudentId == student.Id);

        if (chat is null)
        {
            chat = new Models.Chat()
            {
                AdminId = admin.Id,
                Admin = admin,
                StudentId = student.Id,
                Student = student,
            };

            await db.Chats.AddAsync(chat);
        }

        var message = new Message()
        {
            Content = request.Content,
            AdminId = admin.Id,
            Admin = admin,
            CreatedAt = DateTime.UtcNow,
            IsRead = true,
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

                message.Documents.Add(document);
            }
        }

        chat.Messages.Add(message);

        var sent = await mailService.SendMessageAsync(student.ChatId, message.Content, message.Documents);

        if (!sent)
        {
            return Result.Fail<MessageDto>(ChatErrors.CouldNotSend(student.Email)).ToBadRequestProblem();
        }

        await db.SaveChangesAsync();

        return TypedResults.Ok(chatMapper.Map(message));
    }
}