using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;
using Core.Models.Dto;
using Core.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Core.Handlers;

public static partial class StudentHandler
{
    public static async Task<IResult> GetStudent(Guid id, CoreDb db)
    {
        var activeStudent = db.ActiveStudents.Include(a => a.Admin).SingleOrDefault(a => a.Id == id);

        if (activeStudent == null)
        {
            return Results.NotFound("Student not found");
        }

        await activeStudent.IncludeStudent();
        var dto = ActiveStudentDto.Map(activeStudent);

        return Results.Ok(dto);
    }

    public static async Task<IResult> GetAllStudents(CoreDb db, int pageIndex = 0, int pageSize = -1)
    {
        var activeStudents = db.ActiveStudents.Include(a => a.Admin).ToList();
        await activeStudents.IncludeStudents();

        var dtos = ActiveStudentDto.Maps(activeStudents);
        var paginated = new PaginatedList<ActiveStudentDto>(dtos.ToList(), pageIndex, pageSize);

        return Results.Ok(paginated);
    }

    public static async Task<IResult> GetStudentCourses(Guid id, CoreDb db)
    {
        var activeStudent = db.ActiveStudents.Find(id);

        if (activeStudent == null)
        {
            return Results.NotFound("Could not find the student");
        }

        await activeStudent.IncludeStudent();

        return Results.Ok(activeStudent.Student.OnlineCourse);
    }

    public static async Task<IResult> GetStudentChats(Guid id, CoreDb db, int pageIndex = 0, int pageSize = -1)
    {
        var activeStudent = await db.ActiveStudents.Include(a => a.Admin)
                                .Include(a => a.Chats)
                                .ThenInclude(ch => ch.Admin)
                                .SingleOrDefaultAsync(a => a.Id == id);

        if (activeStudent == null)
        {
            return Results.NotFound("Student not found");
        }

        var dtos = ChatDto.Maps(activeStudent.Chats.ToList());
        var paginated = new PaginatedList<ChatDto>(dtos.ToList(), pageIndex, pageSize);

        return Results.Ok(paginated);
    }

    public static async Task<IResult> GetStudentNotifications(Guid id, CoreDb db, int pageIndex = 0, int pageSize = -1)
    {
        var activeStudent = await db.ActiveStudents.Include(a => a.Admin)
                                .Include(a => a.Notifications)
                                .ThenInclude(n => n.Admin)
                                .Include(a => a.Notifications)
                                .ThenInclude(n => n.Statuses)
                                .Include(a => a.Notifications)
                                .ThenInclude(n => n.Documents)
                                .SingleOrDefaultAsync(a => a.Id == id);

        if (activeStudent == null)
        {
            return Results.NotFound("Could not find student");
        }

        var dtos = NotificationDto.Maps((List<Notification>)activeStudent.Notifications);
        var paginated = new PaginatedList<NotificationDto>(dtos.ToList(), pageIndex, pageSize);

        return Results.Ok(paginated);
    }
}
