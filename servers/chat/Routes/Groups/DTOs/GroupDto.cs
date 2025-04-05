using Notify.Routes.Notifications.DTOs;

namespace Core.Routes.Groups.DTOs;

public class GroupAssignmentDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public AdminDto? Admin { get; set; }
}