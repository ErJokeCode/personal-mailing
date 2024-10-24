using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Shared.Models;

public class Subject
{
    [JsonPropertyName("_id")]
    public string _id { get; set; }
    string FullName { get; set; }
    string Name { get; set; }
    string FormEducation { get; set; }
    string Info { get; set; }
}

public class CourseInfo
{
    [JsonPropertyName("_id")]
    public string _id { get; set; }
    public string Name { get; set; }
    public string University { get; set; }
    public string DateStart { get; set; }
    public string Deadline { get; set; }
    public string Info { get; set; }
}

public class Group
{
    public string Number { get; set; }
    public int NumberCourse { get; set; }
    public string DirectionCode { get; set; }
    public string NameSpeciality { get; set; }
}

public class Student
{
    [JsonPropertyName("_id")]
    public string _id { get; set; }
    public string PersonalNumber { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Patronymic { get; set; }
    public string Email { get; set; }
    public string DateOfBirth { get; set; }
    public Group Group { get; set; }
    public bool Status { get; set; }
    public string TypeOfCost { get; set; }
    public string TypeOfEducation { get; set; }
    public List<Subject> Subjects { get; set; }
    public List<CourseInfo> OnlineCourse { get; set; }
}

public class AdminUser : IdentityUser
{
}

public class ActiveStudent
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string PersonalNumber { get; set; }

    public string StudentId { get; set; }
    public string ChatId { get; set; }

    public ICollection<Notification> Notifications { get; set; }
}

public class Notification
{
    public int Id { get; set; }

    public Guid StudentId { get; set; }
    public ActiveStudent Student { get; set; }

    public string Content { get; set; }
}
