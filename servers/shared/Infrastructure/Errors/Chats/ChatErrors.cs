namespace Shared.Infrastructure.Errors.Chats;

public static class ChatErrors
{
    public static string CouldNotSend(string email) => $"Не смог отправить сообщение студенту с почтой {email}";
}