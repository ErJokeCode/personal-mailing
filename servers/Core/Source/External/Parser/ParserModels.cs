using System.Collections.Generic;
using System.Text.Json;
using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Core.External.Parser;

public static class ParserModelsExtensions
{
    public static void BuildParserModels(this ModelBuilder builder)
    {
        builder.Entity<Student>().OwnsOne(s => s.Info, i =>
        {
            i.OwnsMany(i => i.OnlineCourse, o =>
            {
                o.Property(x => x.Scores).HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
                    v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions)null!)!
                );

            });
        });

        builder.Entity<Student>().OwnsOne(s => s.Info, i =>
        {
            i.OwnsMany(i => i.Subjects, s =>
            {
                s.OwnsOne(s => s.OnlineCourse, o =>
                {
                    o.Property(o => o.Scores).HasConversion(
                        v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
                        v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions)null!)!
                    );
                });
            });
        });
    }
}

[Owned]
public class Team
{
    public required string Name { get; set; }
    public ICollection<string> Teachers { get; set; } = [];
}

[Owned]
public class Subject
{
    public required string FullName { get; set; }
    public required string Name { get; set; }
    public required string FormEducation { get; set; }
    public string? Info { get; set; }
    public CourseInfo? OnlineCourse { get; set; }
    public string? GroupTgLink { get; set; }

    public ICollection<Team> Teams { get; set; } = [];
}

[Owned]
public class CourseInfo
{
    public required string Name { get; set; }
    public string? University { get; set; }
    public string? DateStart { get; set; }
    public string? Deadline { get; set; }
    public string? Info { get; set; }

    public Dictionary<string, object> Scores { get; set; } = [];
}

[Owned]
public class Group
{
    public required string Number { get; set; }
    public int NumberCourse { get; set; }
    public string? DirectionCode { get; set; }
    public string? NameSpeciality { get; set; }
}

[Owned]
public class ParserStudent
{
    public required string PersonalNumber { get; set; }
    public required string Name { get; set; }
    public required string Surname { get; set; }
    public string? Patronymic { get; set; }
    public string? Email { get; set; }
    public required string DateOfBirth { get; set; }
    public required Group Group { get; set; }
    public bool? Status { get; set; }
    public string? TypeOfCost { get; set; }
    public string? TypeOfEducation { get; set; }

    public ICollection<Subject> Subjects { get; set; } = [];
    public ICollection<CourseInfo> OnlineCourse { get; set; } = [];
}