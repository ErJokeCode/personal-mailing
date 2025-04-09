
using System;
using System.Collections.Generic;
using Chatter.Models;
using Chatter.Routes.Documents.DTOs;
using Chatter.Routes.Notifications.DTOs;

namespace Chatter.Routes.Chats.DTOs;

public class MessageDto
{
    public int Id { get; set; }
    public required string Content { get; set; }
    public required DateTime CreatedAt { get; set; }

    public bool IsRead { get; set; } = false;

    public IEnumerable<DocumentDto> Documents { get; set; } = [];
    public AdminDto? Admin { get; set; }
}

public class ChatDto
{
    public int Id { get; set; }
    public int UnreadCount { get; set; }

    public StudentDto? Student { get; set; }
    public AdminDto? Admin { get; set; }

    public IEnumerable<MessageDto> Messages { get; set; } = [];
}