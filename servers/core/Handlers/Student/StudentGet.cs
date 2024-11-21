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
        var activeStudent = db.ActiveStudents.Find(id);

        if (activeStudent == null)
        {
            return Results.NotFound("Student not found");
        }

        await activeStudent.IncludeStudent();
        var dto = ActiveStudentDto.Map(activeStudent);

        return Results.Ok(dto);
    }

    public static async Task<IResult> GetAllStudents(CoreDb db)
    {
        var activeStudents = db.ActiveStudents.ToList();
        await activeStudents.IncludeStudents();

        var dtos = ActiveStudentDto.Maps(activeStudents);

        return Results.Ok(dtos);
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

    public static async Task<IResult> GetStudentChats(Guid id, CoreDb db)
    {
        var activeStudent = await db.ActiveStudents.Include(a => a.Chats)
                                .Include(a => a.Chats)
                                .ThenInclude(ch => ch.Admin)
                                .SingleOrDefaultAsync(a => a.Id == id);

        if (activeStudent == null)
        {
            return Results.NotFound("Student not found");
        }

        return Results.Ok(ChatDto.Maps(activeStudent.Chats.ToList()));
    }

    public static async Task<IResult> GetStudentNotifications(Guid id, CoreDb db)
    {
        var activeStudent = await db.ActiveStudents.Include(a => a.Notifications)
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

        return Results.Ok(dtos);
    }
}
