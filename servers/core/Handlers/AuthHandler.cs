using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Core.Models;
using Microsoft.AspNetCore.Http;
using MassTransit;
using Core.Messages;
using Core.Utility;
using Microsoft.AspNetCore.Identity;
using System.Threading;
using System.Security.Claims;

namespace Core.Handlers;

public static class AuthHandler
{
    public class AuthDetails
    {
        public string Email { get; set; }

        [JsonPropertyName("personal_number")]
        public string PersonalNumber { get; set; }

        [JsonPropertyName("chat_id")]
        public string ChatId { get; set; }
    }

    public static async Task<IResult> AuthStudent(AuthDetails details, IPublishEndpoint endpoint, CoreDb db)
    {
        var activeStudent = db.ActiveStudents.SingleOrDefault(a => a.Email == details.Email);

        if (activeStudent != null)
        {
            await activeStudent.IncludeStudent();
            return Results.Ok(activeStudent);
        }

        activeStudent = new ActiveStudent() {
            Email = details.Email,
            ChatId = details.ChatId,
        };

        var found = await activeStudent.IncludeStudent();

        if (!found || activeStudent.Student.PersonalNumber != details.PersonalNumber)
        {
            return Results.NotFound("Could not find the student");
        }

        db.ActiveStudents.Add(activeStudent);
        await db.SaveChangesAsync();

        await endpoint.Publish<NewStudentAuthed>(new() {
            ActiveStudent = activeStudent,
        });

        return Results.Created<ActiveStudent>("", activeStudent);
    }

    public class AdminDetails
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public static async Task CreateAdmin(string email, string password, UserManager<AdminUser> userManager,
                                         IUserStore<AdminUser> userStore)
    {
        var emailStore = (IUserEmailStore<AdminUser>)userStore;
        var user = new AdminUser();

        await userStore.SetUserNameAsync(user, email, CancellationToken.None);
        await emailStore.SetEmailAsync(user, email, CancellationToken.None);
        var result = await userManager.CreateAsync(user, password);
        await userManager.AddClaimAsync(user, new Claim("Admin", ""));
    }

    public static async Task<IResult> AddNewAdmin(AdminDetails details, HttpContext context,
                                                  UserManager<AdminUser> userManager, IUserStore<AdminUser> userStore,
                                                  CoreDb db)
    {
        await CreateAdmin(details.Email, details.Password, userManager, userStore);
        await db.SaveChangesAsync();
        return Results.Ok();
    }
}
