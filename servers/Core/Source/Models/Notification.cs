
using System;
using System.Collections.Generic;

namespace Core.Models;

public class Notification
{
    public int Id { get; set; }
    public required string Content { get; set; }
    public required DateTime CreatedAt { get; set; }

    public required Guid AdminId { get; set; }
    public Admin? Admin { get; set; }
    public ICollection<Student> Students { get; } = [];
}