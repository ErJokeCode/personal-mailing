using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace Core.Models;

public class CoreDb : DbContext
{
    public DbSet<Student> Students => Set<Student>();

    public CoreDb(DbContextOptions<CoreDb> options) : base(options)
    {
    }
}

public class AuthDetails
{
    [JsonPropertyName("email")]
    public string Email { get; set; }
    [JsonPropertyName("personal_number")]
    public string PersonalNumber { get; set; }
    [JsonPropertyName("chat_id")]
    public string ChatId { get; set; }
}

public class Course
{
    public string Name { get; set; }
    public string University { get; set; }
    public string Score { get; set; }
}

public class UserCourse
{
    [JsonPropertyName("_id")]
    public string _id { get; set; }
    public string Name { get; set; }
    public string Sername { get; set; }
    public string Patronymic { get; set; }

    public List<Course> Courses { get; set; }
}

public class User
{
    [JsonPropertyName("_id")]
    public string _id { get; set; }
    public string PersonalNumber { get; set; }
}

public class Student
{
    public Guid Id { get; set; }

    public string Email { get; set; }
    public string PersonalNumber { get; set; }
    public string ChatId { get; set; }

    public string UserId { get; set; }
    public string UserCourseId { get; set; }
}
