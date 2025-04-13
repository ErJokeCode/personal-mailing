using System;
using System.Threading.Tasks;
using Chatter.Data;
using Chatter.Services.UserAccessor;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Shared.Context.Admins;
using Shared.Context.Groups;
using Shared.Context.Students;

namespace Chatter.Services;

public class ChatService
{
    private readonly AppDbContext _db;
    private readonly IUserAccessor _userAccessor;

    public ChatService(AppDbContext db, IUserAccessor userAccessor)
    {
        _db = db;
        _userAccessor = userAccessor;
    }

    public async Task<Result> CheckAssignedAsync(Guid studentId)
    {
        var student = await _db.Students.FindAsync(studentId);

        if (student is null)
        {
            return Result.Fail(StudentErrors.NotFound(studentId));
        }

        var admin = await _userAccessor.GetUserAsync();

        if (admin is null)
        {
            return Result.Fail(AdminErrors.NotFound());
        }

        var groupAssignment = await _db.GroupAssignments.SingleOrDefaultAsync(g => g.AdminId == admin.Id && g.Name == student.Info.Group.Number);

        if (groupAssignment is null)
        {
            return Result.Fail(GroupErrors.NotAssignedToGroup(admin.Email!, student.Info.Group.Number));
        }

        return Result.Ok();
    }
}