using System;
using System.Threading.Tasks;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Chatter.Data;
using Shared.Context.Admins;
using Shared.Context.Groups;
using Chatter.Services.UserAccessor;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Shared.Context.Students;
using Shared.Infrastructure.Errors;
using Microsoft.AspNetCore.Http;
using Chatter.Services;
using Chatter.Features.Chats.DTOs;

namespace Chatter.Features.Chats.Queries;

public static class GetChatByIdQuery
{
    public static async Task<Results<Ok<ChatDto>, NotFound<ProblemDetails>>> Handle(
        Guid studentId,
        AppDbContext db,
        IUserAccessor userAccessor,
        ChatMapper chatMapper,
        ChatService chatService
    )
    {
        var student = await db.Students.FindAsync(studentId);

        if (student is null)
        {
            return Result.Fail(StudentErrors.NotFound(studentId)).ToNotFoundProblem();
        }

        var admin = await userAccessor.GetUserAsync();

        if (admin is null)
        {
            return Result.Fail(AdminErrors.NotFound()).ToNotFoundProblem();
        }

        var groupAssignment = await db.GroupAssignments.SingleOrDefaultAsync(g => g.AdminId == admin.Id && g.Name == student.Info.Group.Number);

        if (groupAssignment is null)
        {
            return Result.Fail(GroupErrors.NotAssignedToGroup(admin.Email!, student.Info.Group.Number)).ToNotFoundProblem();
        }

        var chat = await db.Chats
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

        return TypedResults.Ok(chatMapper.Map(chat));
    }
}