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

public class SignalHub : Hub
{
}

public class CoreDb : IdentityDbContext<AdminUser>
{
    public DbSet<ActiveStudent> ActiveStudents => Set<ActiveStudent>();
    public DbSet<Notification> Notifications => Set<Notification>();

    public CoreDb(DbContextOptions<CoreDb> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ActiveStudent>().HasMany(e => e.Notifications).WithMany(e => e.ActiveStudents);
    }
}

[Keyless]
public class Subject
{
    [JsonPropertyName("_id")]
    public string _id { get; set; }
    public string FullName { get; set; }
    public string Name { get; set; }
    public string FormEducation { get; set; }
    public string Info { get; set; }
}

[Keyless]
public class CourseInfo
{
    [JsonPropertyName("_id")]
    public string _id { get; set; }
    public string Name { get; set; }
    public string University { get; set; }
    public string DateStart { get; set; }
    public string Deadline { get; set; }
    public string Info { get; set; }
    public string Score { get; set; }
}

[Keyless]
public class Group
{
    public string Number { get; set; }
    public int NumberCourse { get; set; }
    public string DirectionCode { get; set; }
    public string NameSpeciality { get; set; }
}

public class Student
{
    [JsonPropertyName("_id")]
    public string _id { get; set; }
    public string PersonalNumber { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Patronymic { get; set; }
    public string Email { get; set; }
    public string DateOfBirth { get; set; }
    [NotMapped]
    public Group Group { get; set; }
    public bool Status { get; set; }
    public string TypeOfCost { get; set; }
    public string TypeOfEducation { get; set; }
    [NotMapped]
    public List<Subject> Subjects { get; set; }
    [NotMapped]
    public List<CourseInfo> OnlineCourse { get; set; }
}

public class AdminUser : IdentityUser
{
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

    public ICollection<ActiveStudent> ActiveStudents { get; } = [];
}

public class NotificationDto
{
    public int Id { get; set; }
    public string Content { get; set; }
    public string Date { get; set; }
    public ICollection<Guid> StudentIds { get; } = [];
}
