using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Abstractions.FileStorage;
using Core.Abstractions.MailService;
using Core.Abstractions.UserAccesor;
using Core.Data;
using Core.Models;
using Core.Routes.Admins.Errors;
using Core.Routes.Chats.DTOs;
using Core.Routes.Chats.Errors;
using Core.Routes.Chats.Maps;
using Core.Routes.Groups.Errors;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Core.Routes.Chats.Commands;

public class SendMessageCommand : IRequest<Result<MessageDto>>
{
    public required Guid StudentId { get; set; }
    public required string Content { get; set; }
    public required IFormFileCollection FormFiles { get; set; }
}

public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, Result<MessageDto>>
{
    private readonly IUserAccessor _userAccessor;
    private readonly AppDbContext _db;
    private readonly ChatMapper _chatMapper;
    private readonly IMailService _mailService;
    private readonly IFileStorage _fileStorage;

    public SendMessageCommandHandler(IUserAccessor userAccessor, AppDbContext db, IMailService mailService, IFileStorage fileStorage)
    {
        _userAccessor = userAccessor;
        _db = db;
        _chatMapper = new ChatMapper();
        _mailService = mailService;
        _fileStorage = fileStorage;
    }

    public async Task<Result<MessageDto>> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        var admin = await _userAccessor.GetUserAsync();

        if (admin is null)
        {
            return Result.Fail<MessageDto>(AdminErrors.NotFound());
        }

        var student = await _db.Students.SingleAsync(s => s.Id == request.StudentId);

        var groupAssignment = await _db.GroupAssignments.SingleOrDefaultAsync(g => g.AdminId == admin.Id && g.Name == student.Info.Group.Number);

        if (groupAssignment is null || groupAssignment.Name != student.Info.Group.Number)
        {
            return Result.Fail<MessageDto>(GroupErrors.NotAssignedToGroup(admin.Email!, student.Info.Group.Number));
        }

        var chat = await _db.Chats
            .AsSplitQuery()
            .SingleOrDefaultAsync(ch => ch.AdminId == admin.Id && ch.StudentId == student.Id);

        if (chat is null)
        {
            chat = new Chat()
            {
                AdminId = admin.Id,
                Admin = admin,
                StudentId = student.Id,
                Student = student,
            };

            await _db.Chats.AddAsync(chat);
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

                var document = await _fileStorage.UploadAsync(blobData);

                message.Documents.Add(document);
            }
        }

        chat.Messages.Add(message);

        var sent = await _mailService.SendMessageAsync(student.ChatId, message.Content, message.Documents);

        if (!sent)
        {
            return Result.Fail<MessageDto>(ChatErrors.CouldNotSend(student.Email));
        }

        await _db.SaveChangesAsync();

        return _chatMapper.Map(message);
    }
}
