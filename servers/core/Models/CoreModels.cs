using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.SignalR;

namespace Core.Models;

public class AdminUser : IdentityUser
{
    public string Date { get; set; }

    public ICollection<Notification> Notifications { get; } = [];
}

public class ActiveStudent
{
    public Guid Id { get; set; }

    [NotMapped]
    public Student Student { get; set; }

    public string Date { get; set; }
    public string Email { get; set; }
    public string ChatId { get; set; }

    public List<string> OnboardStatus { get; set; } = [];

    public ICollection<Notification> Notifications { get; } = [];
}

public class Notification
{
    public int Id { get; set; }

    public string Content { get; set; }
    public string Date { get; set; }

    public string AdminId { get; set; }
    public AdminUser Admin { get; set; }
    public ICollection<ActiveStudent> ActiveStudents { get; } = [];

    public ICollection<Document> Documents { get; } = [];
}

public class Document
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string MimeType { get; set; }
    public string InternalName { get; set; }

    public int NotificationId { get; set; }
    public Notification Notification { get; set; }
}
