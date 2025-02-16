using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Core.Models;

// Try to make a forum like system, where individual messages have an admin that sent it
// Chats could be transfered to a new admin, if groups are reassigned, to preserve the context
// ?? Maybe even allow for all admins to send a message in the chat

// POST /core/chats?fromStudent=id&?toStudent=id
// GET /core/chats chats for this admin
// GET /core/chats/{studentId} chat with a specific student, automatically makes chat read
// Return an empty chat if admin is assigned to the student, return forbidden if not

[Owned]
public class Message
{
    public required string Content { get; set; }
    public required DateTime CreatedAt { get; set; }

    public Guid? AdminId { get; set; }
    public Admin? Admin { get; set; }
}

public class Chat
{
    public int Id { get; set; }
    public int UnreadCount { get; set; }

    public required Guid StudentId { get; set; }
    public Student? Student { get; set; }

    public required Guid AdminId { get; set; }
    public Admin? Admin { get; set; }

    public ICollection<Message> Messages { get; set; } = [];
}