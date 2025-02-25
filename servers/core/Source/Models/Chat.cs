using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Riok.Mapperly.Abstractions;

namespace Core.Models;

// Try to make a forum like system, where individual messages have an admin that sent it
// Chats could be transfered to a new admin, if groups are reassigned, to preserve the context
// ?? Maybe even allow for all admins to send a message in the chat

// POST /core/chats send message to student
// Post /core/chats/from-student
// GET /core/chats chats for this admin
// GET /core/chats/{studentId} chat with a specific student, automatically makes chat read
// Return an empty chat if admin is assigned to the student, return forbidden if not

// Make not owned but separate entity, make separate GetMessage endpoint

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