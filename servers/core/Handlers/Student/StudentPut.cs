using System;
using System.Collections.Generic;
using System.Linq;
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
        public List<string> Groups { get; set; } = [];
    }

    // public static async Task<IResult> ChangeAdmin(Guid id, AdminDetails details, CoreDb db)
    // {
    //     var activeStudent = db.ActiveStudents.Find(id);
    //
    //     if (activeStudent == null)
    //     {
    //         return Results.NotFound("Student not found");
    //     }
    //
    //     var found = await activeStudent.IncludeStudent();
    //
    //     if (!found)
    //     {
    //         return Results.NotFound("Student not found");
    //     }
    //
    //     var admin = db.Users.SingleOrDefault(a => a.Groups.Contains(activeStudent.Student.Group.Number));
    //
    //     if (admin == null)
    //     {
    //         return Results.NotFound("Admin not found");
    //     }
    //
    //     admin.Groups.Clear();
    //     admin.Groups.AddRange(details.Groups);
    //
    //     await db.SaveChangesAsync();
    //
    //     return Results.Ok();
    // }
}
