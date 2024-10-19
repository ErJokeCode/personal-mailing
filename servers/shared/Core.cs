using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Shared.Core;

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
