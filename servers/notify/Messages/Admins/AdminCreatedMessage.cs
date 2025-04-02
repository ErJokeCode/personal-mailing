using System;

namespace Notify.Messages.Admins;

public class AdminDto
{
    public Guid Id { get; set; }
    public required string UserName { get; set; }
    public required string Email { get; set; }
    public required DateOnly CreatedAt { get; set; }
}

public class AdminCreatedMessage
{
    public required AdminDto Admin { get; set; }
}