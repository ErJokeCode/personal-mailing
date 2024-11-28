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

    public class AdminDetails
    {
        public string AdminId { get; set; }
    }

    public static async Task<IResult> ChangeAdmin(Guid id, AdminDetails details, CoreDb db)
    {
        var activeStudent = db.ActiveStudents.Find(id);

        if (activeStudent == null)
        {
            return Results.NotFound("Student not found");
        }

        var admin = db.Users.Find(details.AdminId);

        if (admin == null)
        {
            return Results.NotFound("Admin not found");
        }

        activeStudent.AdminId = details.AdminId;

        await db.SaveChangesAsync();

        return Results.Ok();
    }
}
