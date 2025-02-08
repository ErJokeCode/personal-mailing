using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace Core.External.Parser;

[Keyless]
public class Team
{
    public required string Name { get; set; }
    public List<string> Teachers { get; set; } = [];
}

[Keyless]
public class Subject
{
    public required string FullName { get; set; }
    public required string Name { get; set; }
    public required string FormEducation { get; set; }
    public string? Info { get; set; }
    public CourseInfo? OnlineCourse { get; set; }
    public string? GroupTgLink { get; set; }
    public List<Team> Teams { get; set; } = [];
}

[Keyless]
public class CourseInfo
{
    public required string Name { get; set; }
    public string? University { get; set; }
    public string? DateStart { get; set; }
    public string? Deadline { get; set; }
    public string? Info { get; set; }
    public Dictionary<string, object> Scores { get; set; } = [];
}

[Keyless]
public class Group
{
    public required string Number { get; set; }
    public int NumberCourse { get; set; }
    public string? DirectionCode { get; set; }
    public string? NameSpeciality { get; set; }
}

public class ParserStudent
{
    [JsonPropertyName("_id")]
    public string? _id { get; set; }
    public required string PersonalNumber { get; set; }
    public required string Name { get; set; }
    public required string Surname { get; set; }
    public string? Patronymic { get; set; }
    public string? Email { get; set; }
    public required string DateOfBirth { get; set; }
    [NotMapped]
    public required Group Group { get; set; }
    public bool? Status { get; set; }
    public string? TypeOfCost { get; set; }
    public string? TypeOfEducation { get; set; }
    [NotMapped]
    public List<Subject> Subjects { get; set; } = [];
    [NotMapped]
    public List<CourseInfo> OnlineCourse { get; set; } = [];
}