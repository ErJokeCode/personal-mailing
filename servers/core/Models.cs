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
    public DbSet<CourseInfo> CourseInfo => Set<CourseInfo>();
    public DbSet<Subject> Subject => Set<Subject>();
    public DbSet<Group> Group => Set<Group>();

    public CoreDb(DbContextOptions<CoreDb> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}

public class ObjectId
{
    [JsonPropertyName("$oid")]
    public string Value { get; set; }
}

public class Subject
{
    public int Id { get; set; }
    public Guid ActiveStudentId { get; set; }

    public string FullName { get; set; }
    public string Name { get; set; }
    public string FormEducation { get; set; }
    public string Info { get; set; }
}

public class CourseInfo
{
    public int Id { get; set; }
    public Guid ActiveStudentId { get; set; }

    public string Name { get; set; }
    public string University { get; set; }
    public string DateStart { get; set; }
    public string Deadline { get; set; }
    public string Info { get; set; }
    public string Score { get; set; }
}

public class Group
{
    public int Id { get; set; }
    public Guid ActiveStudentId { get; set; }

    public string Number { get; set; }
    public int NumberCourse { get; set; }
    public string DirectionCode { get; set; }
    public string NameSpeciality { get; set; }
}

public abstract class BaseStudent
{
    public string PersonalNumber { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Patronymic { get; set; }
    public string Email { get; set; }
    public string DateOfBirth { get; set; }
    public Group Group { get; set; }
    public bool Status { get; set; }
    public string TypeOfCost { get; set; }
    public string TypeOfEducation { get; set; }
    public List<Subject> Subjects { get; set; }
    public List<CourseInfo> OnlineCourse { get; set; }
}

public class Student : BaseStudent
{
    [JsonPropertyName("_id")]
    public ObjectId _id { get; set; }
}

public class AdminUser : IdentityUser
{
}

public class ActiveStudent : BaseStudent
{
    public Guid Id { get; set; }

    public string ChatId { get; set; }
    public ICollection<Notification> Notifications { get; set; }
}

public class Notification
{
    public int Id { get; set; }

    public Guid ActiveStudentId { get; set; }
    public ActiveStudent ActiveStudent { get; set; }

    public string Content { get; set; }
    public string Date { get; set; }
}
