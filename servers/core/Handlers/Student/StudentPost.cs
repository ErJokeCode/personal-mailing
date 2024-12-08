using System;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Core.Models;
using Core.Models.Dto;
using Core.Utility;
using Microsoft.AspNetCore.Http;

namespace Core.Handlers;

public static partial class StudentHandler
{
    public class AuthDetails
    {
        public string Email { get; set; }
        public string PersonalNumber { get; set; }
        public string ChatId { get; set; }
    }

    public static async Task<IResult> AuthStudent(AuthDetails details, CoreDb db)
    {
        var activeStudent = db.ActiveStudents.SingleOrDefault(a => a.Email == details.Email);

        if (activeStudent != null)
        {
            await activeStudent.IncludeStudent();
            return Results.Ok(activeStudent);
        }

        var admin = db.Users.FirstOrDefault();

        activeStudent = new ActiveStudent()
        {
            Email = details.Email,
            ChatId = details.ChatId,
            Date = DateTime.Now.ToString(),
            AdminId = admin.Id
        };

        var found = await activeStudent.IncludeStudent();

        if (!found || activeStudent.Student.PersonalNumber != details.PersonalNumber)
        {
            return Results.NotFound("Could not find the student");
        }

        db.ActiveStudents.Add(activeStudent);
        await db.SaveChangesAsync();

        var dto = ActiveStudentDto.Map(activeStudent);
        return Results.Created("", dto);
    }
}
