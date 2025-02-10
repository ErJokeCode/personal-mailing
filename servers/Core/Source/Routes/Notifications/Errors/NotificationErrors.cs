namespace Core.Routes.Notifications.Errors;

public static class NotificationErrors
{
    public static string NotFound(int id) => $"Рассылка с айди {id} не была найдена";
}