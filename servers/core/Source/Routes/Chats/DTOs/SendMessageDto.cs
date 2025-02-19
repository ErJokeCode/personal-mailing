using System;

namespace Core.Routes.Chats.DTOs;

public class SendMessageDto
{
    public required string Content { get; set; }
    public required Guid StudentId { get; set; }
}