using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Core.Models;

[Keyless]
public class Subject
{
    [JsonPropertyName("_id")]
    public string _id { get; set; }
    public string FullName { get; set; }
    public string Name { get; set; }
    public string FormEducation { get; set; }
    public string Info { get; set; }
    public string OnlineCourse { get; set; }
    public string GroupTgLink { get; set; }
}

[Keyless]
public class CourseInfo
{
    [JsonPropertyName("_id")]
    public string _id { get; set; }
    public string Name { get; set; }
    public string University { get; set; }
    public string DateStart { get; set; }
    public string Deadline { get; set; }
    public string Info { get; set; }
    public Dictionary<string, object> Scores { get; set; }
}

[Keyless]
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
    [NotMapped]
    public Group Group { get; set; }
    public bool Status { get; set; }
    public string TypeOfCost { get; set; }
    public string TypeOfEducation { get; set; }
    [NotMapped]
    public List<Subject> Subjects { get; set; }
    [NotMapped]
    public List<CourseInfo> OnlineCourse { get; set; }
}
