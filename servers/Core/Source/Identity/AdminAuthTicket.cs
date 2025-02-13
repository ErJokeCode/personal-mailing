using System;
using Core.Models;

namespace Core.Identity;

public class AdminAuthTicket
{
    public Guid Id { get; set; }

    public Guid AdminId { get; set; }
    public Admin? Admin { get; set; }

    public required byte[] Value { get; set; }

    public DateTimeOffset? Expires { get; set; }
}