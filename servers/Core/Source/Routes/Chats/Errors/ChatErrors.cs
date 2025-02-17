
namespace Core.Routes.Chats.Errors;

public static class ChatErrors
{
    public static string CouldNotSend(string email) => $"Не смог отправить сообщение студенту с почтой {email}";
}