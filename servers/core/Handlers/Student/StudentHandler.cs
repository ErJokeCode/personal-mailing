using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Models;

namespace Core.Handlers;

public static partial class StudentHandler
{
}

public static class ActiveStudentExtensions
{
    public static async Task<bool> IncludeStudent(this ActiveStudent active)
    {
        var query = new Dictionary<string, string>
        {
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
