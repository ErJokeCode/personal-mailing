using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Chatter.Models;
using Chatter.Routes.Chats.DTOs;
using Chatter.Routes.Chats.Maps;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Chatter.Abstractions.MailService;
using Chatter.Abstractions.UserAccessor;
using Chatter.Data;
using Shared.Services.FileStorage;
using Shared.Context.Admins;
using Shared.Context.Chats;
using Shared.Context.Groups;

namespace Chatter.Routes.Chats.Commands;

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
        var adminInfo = await _userAccessor.GetUserAsync();

        if (adminInfo is null)
        {
            return Result.Fail<MessageDto>(AdminErrors.NotFound());
        }

        var admin = await _db.Users.FindAsync(adminInfo.Id);

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
            chat = new Models.Chat()
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
