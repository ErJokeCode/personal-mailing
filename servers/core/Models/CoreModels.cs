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
    public ICollection<Notification> Notifications { get; } = [];
}

public class ActiveStudent
{
    public Guid Id { get; set; }

    [NotMapped]
    public Student Student { get; set; }

    public string Email { get; set; }
    public string ChatId { get; set; }

    public ICollection<Notification> Notifications { get; } = [];
}

public class Notification
{
    public int Id { get; set; }

    public string Content { get; set; }
    public string Date { get; set; }
    public List<string> FileNames { get; set; } = [];

    public string AdminId { get; set; }
    public AdminUser Admin { get; set; }
    public ICollection<ActiveStudent> ActiveStudents { get; } = [];
}
