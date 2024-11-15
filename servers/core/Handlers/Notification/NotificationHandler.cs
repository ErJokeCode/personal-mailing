using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Models;

namespace Core.Handlers;

public static partial class NotificationHandler
{
    public static Dictionary<string, Func<string, ActiveStudent, string>> Passes =
        new() { { "$name", (content, student) =>
                           { return content.Replace("$name", student.Student.Name); } },

                { "$email", (content, student) =>
                            { return content.Replace("$email", student.Email); } }

        };

    public static async Task<string> FillTemplate(string content, ActiveStudent student)
    {
        if (student.Student == null)
        {
            await student.IncludeStudent();
        }

        foreach (var pass in Passes)
        {
            content = pass.Value(content, student);
        }

        return content;
    }
}
