using System;
using System.Threading;
using System.Threading.Tasks;
using Chatter.Models;
using Chatter.Routes.Chats.DTOs;
using Chatter.Routes.Chats.Maps;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Chatter.Abstractions.UserAccessor;
using Chatter.Data;
using Shared.Context.Admins;
using Shared.Context.Groups;

public class GetChatById : IRequest<Result<ChatDto>>
{
    public required Guid StudentId { get; set; }
}

public class GetChatByIdHandler : IRequestHandler<GetChatById, Result<ChatDto>>
{
    private readonly AppDbContext _db;
    private readonly IUserAccessor _userAccessor;
    private readonly ChatMapper _chatMapper;

    public GetChatByIdHandler(AppDbContext db, IUserAccessor userAccessor)
    {
        _db = db;
        _userAccessor = userAccessor;
        _chatMapper = new ChatMapper();
    }

    public async Task<Result<ChatDto>> Handle(GetChatById request, CancellationToken cancellationToken)
    {
        var adminInfo = await _userAccessor.GetUserAsync();

        if (adminInfo is null)
        {
            return Result.Fail<ChatDto>(AdminErrors.NotFound());
        }

        var admin = await _db.Users.FindAsync(adminInfo.Id);

        if (admin is null)
        {
            return Result.Fail<ChatDto>(AdminErrors.NotFound());
        }

        var student = await _db.Students.SingleAsync(s => s.Id == request.StudentId);

        var groupAssignment = await _db.GroupAssignments.SingleOrDefaultAsync(g => g.AdminId == admin.Id && g.Name == student.Info.Group.Number);

        if (groupAssignment is null)
        {
            return Result.Fail<ChatDto>(GroupErrors.NotAssignedToGroup(admin.Email!, student.Info.Group.Number));
        }

        var chat = await _db.Chats
            .AsSplitQuery()
            .SingleOrDefaultAsync(ch => ch.AdminId == admin.Id && ch.StudentId == student.Id);

        if (chat is null)
        {
            chat = new Chatter.Models.Chat()
            {
                AdminId = admin.Id,
                Admin = admin,
                StudentId = student.Id,
                Student = student,
            };
        }

        return _chatMapper.Map(chat);
    }
}
