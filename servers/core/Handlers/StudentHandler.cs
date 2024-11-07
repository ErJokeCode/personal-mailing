using System;
using System.Collections.Generic;
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

public static class StudentHandler
{
    public class AuthDetails
    {
        public string Email { get; set; }

        [JsonPropertyName("personal_number")]
        public string PersonalNumber { get; set; }

        [JsonPropertyName("chat_id")]
        public string ChatId { get; set; }
    }

    public static async Task<IResult> AuthStudent(AuthDetails details, IPublishEndpoint endpoint, CoreDb db)
    {
        var activeStudent = db.ActiveStudents.SingleOrDefault(a => a.Email == details.Email);

        if (activeStudent != null)
        {
            await activeStudent.IncludeStudent();
            return Results.Ok(activeStudent);
        }

        activeStudent =
            new ActiveStudent() { Email = details.Email, ChatId = details.ChatId, Date = DateTime.Now.ToString() };

        var found = await activeStudent.IncludeStudent();

        if (!found || activeStudent.Student.PersonalNumber != details.PersonalNumber)
        {
            return Results.NotFound("Could not find the student");
        }

        db.ActiveStudents.Add(activeStudent);
        await db.SaveChangesAsync();

        await endpoint.Publish<NewStudentAuthed>(new() {
            ActiveStudent = activeStudent,
        });

        var dto = ActiveStudentDto.Map(activeStudent);
        return Results.Created("", dto);
    }

    public static async Task<IResult> AddOnboardStatus(Guid id, string status, CoreDb db)
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

        activeStudent.OnboardStatus.Add(status);

        await db.SaveChangesAsync();

        return Results.Ok(activeStudent.OnboardStatus);
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

    public static async Task<IResult> AddCuratorChat(Guid id, string chatId, CoreDb db)
    {
        var activeStudent = db.ActiveStudents.Find(id);

        if (activeStudent == null)
        {
            return Results.NotFound("Student not found");
        }

        activeStudent.AdminChatId = chatId;

        db.SaveChangesAsync();

        return Results.Ok();
    }
}

public static class ActiveStudentExtensions
{
    public static async Task<bool> IncludeStudent(this ActiveStudent active)
    {
        var query = new Dictionary<string, string> {
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
