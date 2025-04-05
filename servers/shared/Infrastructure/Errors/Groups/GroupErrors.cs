namespace Shared.Infrastructure.Errors.Groups;

public static class GroupErrors
{
    public static string NotAssignedToGroup(string email, string group) => $"Админ с почтой {email} не привязан к группе {group}";
}