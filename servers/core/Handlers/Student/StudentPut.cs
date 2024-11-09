using System;
using System.Threading.Tasks;
using Core.Utility;
using Microsoft.AspNetCore.Http;

namespace Core.Handlers;

public static partial class StudentHandler
{
    public class OnboardDetails
    {
        public string Status { get; set; }
    }

    public static async Task<IResult> AddOnboardStatus(Guid id, OnboardDetails details, CoreDb db)
    {
        var activeStudent = db.ActiveStudents.Find(id);

        if (activeStudent == null)
        {
            return Results.NotFound("Could not find student");
        }

        if (activeStudent.OnboardStatus == null)
        {
            activeStudent.OnboardStatus = new();
        }

        activeStudent.OnboardStatus.Add(details.Status);

        await db.SaveChangesAsync();

        return Results.Ok(activeStudent.OnboardStatus);
    }

    public class ChatDetails
    {
        public string ChatId { get; set; }
    }

    public static async Task<IResult> AddCuratorChat(Guid id, ChatDetails details, CoreDb db)
    {
        var activeStudent = db.ActiveStudents.Find(id);

        if (activeStudent == null)
        {
            return Results.NotFound("Student not found");
        }

        activeStudent.AdminChatId = details.ChatId;

        await db.SaveChangesAsync();

        return Results.Ok();
    }
}
