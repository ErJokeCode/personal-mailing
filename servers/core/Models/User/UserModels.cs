using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Core.Models;

public class AdminUser : IdentityUser
{
    public string Date { get; set; }
    public List<string> Permissions { get; set; } = [];

    public ICollection<Notification> Notifications { get; } = [];
    public ICollection<NotificationTemplate> Templates { get; } = [];
    public ICollection<NotificationSchedule> Schedules { get; } = [];
    public ICollection<Chat> Chats { get; } = [];
}

public class ActiveStudent
{
    public Guid Id { get; set; }

    [NotMapped]
    public Student Student { get; set; }

    public string Date { get; set; }
    public string Email { get; set; }
    public string ChatId { get; set; }
    public string AdminChatId { get; set; }

    public List<string> OnboardStatus { get; set; } = [];

    public ICollection<Notification> Notifications { get; } = [];
    public ICollection<Chat> Chats { get; } = [];
    public ICollection<NotificationTemplate> Templates { get; } = [];
}
