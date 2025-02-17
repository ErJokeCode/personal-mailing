namespace Core.Routes.Groups.Errors;

public static class GroupErrors
{
    public static string NotAssignedToGroup(string email, string group) => $"Админ с почтой {email} не привязан к группе {group}";
}