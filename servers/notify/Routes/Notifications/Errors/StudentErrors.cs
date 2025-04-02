using System;

namespace Notify.Routes.Notifications.Errors;

public static class StudentErrors
{
    public static string NotFound(Guid id) => $"Студент с айди {id} не был найден или он больше не активен";

    public static string AuthError() => "Ошибка аутентификации";
}