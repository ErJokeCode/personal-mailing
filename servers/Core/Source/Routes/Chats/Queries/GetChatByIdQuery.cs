
using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Abstractions.UserAccesor;
using Core.Data;
using Core.Models;
using Core.Routes.Admins.Errors;
using Core.Routes.Chats.DTOs;
using Core.Routes.Chats.Maps;
using Core.Routes.Groups.Errors;
using Core.Routes.Students.Errors;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class GetChatById : IRequest<Result<ChatDto>>
{
    public Guid StudentId { get; set; }
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
        var admin = await _userAccessor.GetUserAsync();

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
            chat = new Chat()
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
