using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Core.Models;

namespace Core.Handlers;

public static partial class StudentHandler
{
    private static int FindLowEnd(string value)
    {
        var match = Regex.Match(value, @"\(проходной балл (\d+)\)");
        int.TryParse(match.Groups[1].Value, out int score);
        return score;
    }

    public static bool AnyLowScore(CourseInfo course)
    {
        return course.Scores.Any(s => int.TryParse(s.Value.ToString(), out int score) &&
                                      score < FindLowEnd(s.Key.ToString()));
    }

    public static bool AnyNotOnCourse(CourseInfo course)
    {
        return course.Scores.Any(s => s.Value.ToString() == "Нет на курсе");
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
