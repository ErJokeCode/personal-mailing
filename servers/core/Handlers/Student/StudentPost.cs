using System;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Core.Messages;
using Core.Models;
using Core.Models.Dto;
using Core.Utility;
using MassTransit;
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

    public static async Task<IResult> AuthStudent(AuthDetails details, CoreDb db, IPublishEndpoint endpoint)
    {
        var activeStudent = db.ActiveStudents.SingleOrDefault(a => a.Email == details.Email);

        if (activeStudent != null)
        {
            await activeStudent.IncludeStudent();
            return Results.Ok(activeStudent);
        }

        activeStudent = new ActiveStudent()
        {
            Email = details.Email,
            ChatId = details.ChatId,
            Date = DateTime.Now.ToString(),
        };

        var found = await activeStudent.IncludeStudent();

        if (!found || activeStudent.Student.PersonalNumber != details.PersonalNumber)
        {
            return Results.NotFound("Could not find the student");
        }

        var admin = db.Users.SingleOrDefault(a => a.Groups.Contains(activeStudent.Student.Group.Number));

        if (admin == null)
        {
            admin = db.Users.MinBy(a => DateTime.Parse(a.Date));
            admin.Groups.Add(activeStudent.Student.Group.Number);
        }

        db.ActiveStudents.Add(activeStudent);
        await db.SaveChangesAsync();

        await endpoint.Publish(new NewActiveStudent()
        {
            ActiveStudent = ActiveStudentDto.Map(activeStudent),
        });

        var dto = ActiveStudentDto.Map(activeStudent);
        return Results.Created("", dto);
    }
}
