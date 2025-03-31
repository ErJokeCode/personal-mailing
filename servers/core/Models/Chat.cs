using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Riok.Mapperly.Abstractions;

namespace Core.Models;

public class Message
{
    public int Id { get; set; }
    public required string Content { get; set; }
    public required DateTime CreatedAt { get; set; }
    public bool IsRead { get; set; } = false;

    public ICollection<Document> Documents { get; set; } = [];

    [MapperIgnore]
    public int ChatId { get; set; }
    [MapperIgnore]
    public Chat? Chat { get; set; }

    [MapperIgnore]
    public Guid? AdminId { get; set; }
    public Admin? Admin { get; set; }
}

public class Chat
{
    public int Id { get; set; }
    public int UnreadCount { get; set; }

    [MapperIgnore]
    public required Guid StudentId { get; set; }
    public Student? Student { get; set; }

    [MapperIgnore]
    public required Guid AdminId { get; set; }
    public Admin? Admin { get; set; }

    public ICollection<Message> Messages { get; set; } = [];
}