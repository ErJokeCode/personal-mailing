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

    public static async Task<IResult> GetAllStudents(CoreDb db, bool notOnCourse = false, bool lowScore = false,
                                                     string group = null, int course = 0, string typeOfCost = null,
                                                     string typeOfEducation = null, string onlineCourse = null,
                                                     string subject = null, int pageIndex = 0, int pageSize = -1)
    {
        var activeStudents = db.ActiveStudents.Include(a => a.Admin).ToList();
        await activeStudents.IncludeStudents();

        if (lowScore && notOnCourse)
        {
            return Results.BadRequest($"Can not filter by both {nameof(notOnCourse)} and {nameof(lowScore)}");
        }
        else if (notOnCourse)
        {
            activeStudents =
                activeStudents.Where(a => a.Student.OnlineCourse.Any(StudentHandler.AnyNotOnCourse)).ToList();
        }
        else if (lowScore)
        {
            activeStudents = activeStudents.Where(a => a.Student.OnlineCourse.Any(StudentHandler.AnyLowScore)).ToList();
        }

        if (!string.IsNullOrEmpty(group))
        {
            activeStudents =
                activeStudents.Where(a => a.Student.Group.Number.ToLower().Contains(group.ToLower())).ToList();
        }

        if (course > 0)
        {
            activeStudents = activeStudents.Where(a => a.Student.Group.NumberCourse == course).ToList();
        }

        if (!string.IsNullOrEmpty(typeOfCost))
        {
            activeStudents =
                activeStudents.Where(a => a.Student.TypeOfCost.ToLower().Contains(typeOfCost.ToLower())).ToList();
        }

        if (!string.IsNullOrEmpty(typeOfEducation))
        {
            activeStudents =
                activeStudents.Where(a => a.Student.TypeOfEducation.ToLower().Contains(typeOfEducation.ToLower()))
                    .ToList();
        }

        if (!string.IsNullOrEmpty(onlineCourse))
        {
            activeStudents =
                activeStudents
                    .Where(a => a.Student.OnlineCourse.Any(o => o.Name.ToLower().Contains(onlineCourse.ToLower())))
                    .ToList();
        }

        if (!string.IsNullOrEmpty(subject))
        {
            activeStudents =
                activeStudents.Where(a => a.Student.Subjects.Any(s => s.FullName.ToLower().Contains(subject.ToLower())))
                    .ToList();
        }

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
