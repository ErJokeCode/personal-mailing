using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models;

public class Chat
{
    public int Id { get; set; }
    public int UnreadCount { get; set; }

    public string AdminId { get; set; }
    public AdminUser Admin { get; set; }

    public Guid ActiveStudentId { get; set; }
    public ActiveStudent ActiveStudent { get; set; }

    public ICollection<Message> Messages { get; } = [];
}

public class Message
{
    public int Id { get; set; }

    public string Date { get; set; }
    public string Content { get; set; }

    public string Sender { get; set; }
    public string Receiver { get; set; }

    public List<int> DocumentIds { get; set; } = [];
    [NotMapped]
    public List<Document> Documents { get; } = [];

    public int StatusId { get; set; }
    public MessageStatus Status { get; set; }

    public int ChatId { get; set; }
    public Chat Chat { get; set; }
}

public class MessageStatus : BaseStatus
{
    public int MessageId { get; set; }
    public Message Message { get; set; }
}
