using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Models;
using Core.Routes.Chats.DTOs;
using Core.Routes.Chats.Maps;
using Core.Signal;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Notify.Data;
using Shared.Abstractions.FileStorage;

namespace Core.Routes.Chats.Commands;

public class SendMessageFromStudentCommand : IRequest<Result<MessageDto>>
{
    public required Guid StudentId { get; set; }
    public required string Content { get; set; }
    public required IFormFileCollection FormFiles { get; set; }
}

public class SendMessageFromStudentCommandHandler : IRequestHandler<SendMessageFromStudentCommand, Result<MessageDto>>
{
    private readonly AppDbContext _db;
    private readonly ChatMapper _chatMapper;
    private readonly IFileStorage _fileStorage;
    private readonly IHubContext<SignalHub> _hub;

    public SendMessageFromStudentCommandHandler(AppDbContext db, IFileStorage fileStorage, IHubContext<SignalHub> hub)
    {
        _db = db;
        _chatMapper = new ChatMapper();
        _fileStorage = fileStorage;
        _hub = hub;
    }

    public async Task<Result<MessageDto>> Handle(SendMessageFromStudentCommand request, CancellationToken cancellationToken)
    {
        var student = await _db.Students.SingleAsync(s => s.Id == request.StudentId);

        var groupAssignment = await _db.GroupAssignments.SingleAsync(g => g.Name == student.Info.Group.Number);

        var admin = await _db.Users.SingleAsync(a => a.Id == groupAssignment.AdminId);

        var chat = await _db.Chats
            .AsSplitQuery()
            .SingleOrDefaultAsync(ch => ch.AdminId == admin.Id && ch.StudentId == student.Id);

        if (chat is null)
        {
            chat = new Core.Models.Chat()
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
            CreatedAt = DateTime.UtcNow,
            IsRead = false,
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
        chat.UnreadCount += 1;

        await _db.SaveChangesAsync();

        var dto = _chatMapper.Map(message);
        await _hub.NotifyOfMessage(admin.Id, dto);

        return dto;
    }
}
