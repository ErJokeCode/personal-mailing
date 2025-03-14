namespace Core.Routes.Notifications.Errors;

public static class NotificationErrors
{
    public static string NotFound(int id) => $"Рассылка с айди {id} не была найдена";

    public static string CouldNotSend(string email) => $"Не смог отправить рассылку студенту с почтой {email}";
}