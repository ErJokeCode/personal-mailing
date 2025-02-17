
using System;
using System.Collections.Generic;
using Core.Models;
using Core.Routes.Admins.Dtos;
using Core.Routes.Documents.DTOs;
using Core.Routes.Students.Dtos;

namespace Core.Routes.Chats.DTOs;

public class MessageDto
{
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