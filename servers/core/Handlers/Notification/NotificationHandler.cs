using System.Threading.Tasks;
using Core.Models;

namespace Core.Handlers;

public static partial class NotificationHandler
{
    public static async Task<string> FillTemplate(string content, ActiveStudent student)
    {
        if (student.Student == null)
        {
            await student.IncludeStudent();
        }

        return content.Replace("$name", student.Student.Name).Replace("$email", student.Email);
    }
}
