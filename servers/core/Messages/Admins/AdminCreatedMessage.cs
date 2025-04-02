using Core.Routes.Admins.Dtos;

namespace Core.Messages.Admins;

public class AdminCreatedMessage
{
    public required AdminDto Admin { get; set; }
}