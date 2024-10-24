using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Shared.Models;

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

public class AdminUser : IdentityUser
{
}

public class Student
{
    public Guid Id { get; set; }

    public string Email { get; set; }
    public string PersonalNumber { get; set; }
    public string ChatId { get; set; }

    public string UserId { get; set; }
    public string UserCourseId { get; set; }

    public ICollection<Notification> Notifications { get; set; }
}

public class Notification
{
    public int Id { get; set; }

    public Guid StudentId { get; set; }
    public Student Student { get; set; }

    public string Content { get; set; }
}
