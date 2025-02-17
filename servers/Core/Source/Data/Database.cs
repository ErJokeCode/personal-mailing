using System;
using Core.Abstractions.Parser;
using Core.Identity;
using Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Core.Data;

public class AppDbContext : IdentityDbContext<Admin, IdentityRole<Guid>, Guid>
{
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<AdminAuthTicket> AdminAuthTickets => Set<AdminAuthTicket>();
    public DbSet<GroupAssignment> GroupAssignments => Set<GroupAssignment>();
    public DbSet<Chat> Chats => Set<Chat>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.BuildParserModels();
    }
}