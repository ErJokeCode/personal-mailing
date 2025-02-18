
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Riok.Mapperly.Abstractions;

namespace Core.Models;

[Owned]
public class NotificationError
{
    public Guid StudentId { get; set; }
    public required string Message { get; set; }
}

public class Notification
{
    public int Id { get; set; }
    public required string Content { get; set; }
    public required DateTime CreatedAt { get; set; }

    public ICollection<Document> Documents { get; set; } = [];

    [MapperIgnore]
    public required Guid AdminId { get; set; }
    public Admin? Admin { get; set; }
    public ICollection<Student> Students { get; set; } = [];

    public ICollection<NotificationError> Errors { get; set; } = [];
}