using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;
using Core.Utility;
using Microsoft.AspNetCore.Http;

namespace Core.Handlers;

public static class StudentHandler
{
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

    public static async Task<IResult> GetStudents(CoreDb db)
    {
        var activeStudents = db.ActiveStudents.ToArray();
        await activeStudents.IncludeStudents();

        return Results.Ok(activeStudents);
    }
}

public static class ActiveStudentExtensions
{
    public static async Task<bool> IncludeStudent(this ActiveStudent active)
    {
        var query = new Dictionary<string, string>
        {
            ["email"] = active.Email,
        };

        var student = await ParserHandler.GetFromParser<Student>("/student", query);

        if (student == null)
        {
            return false;
        }

        active.Student = student;
        return true;
    }

    public static async Task IncludeStudents(this ICollection<ActiveStudent> actives)
    {
        foreach (var active in actives)
        {
            await active.IncludeStudent();
        }
    }
}
