using Core.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Core.Utility;

public class SignalHub : Hub
{
}

public class CoreDb : IdentityDbContext<AdminUser>
{
    public DbSet<ActiveStudent> ActiveStudents => Set<ActiveStudent>();

    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<NotificationStatus> NotificationStatuses => Set<NotificationStatus>();
    public DbSet<Document> Documents => Set<Document>();

    public DbSet<Chat> Chats => Set<Chat>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<MessageStatus> MessageStatuses => Set<MessageStatus>();

    public CoreDb(DbContextOptions<CoreDb> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ActiveStudent>().HasMany(e => e.Notifications).WithMany(e => e.ActiveStudents);

        modelBuilder.Entity<AdminUser>()
            .HasMany(e => e.Notifications)
            .WithOne(e => e.Admin)
            .HasForeignKey(e => e.AdminId);

        modelBuilder.Entity<Notification>()
            .HasMany(e => e.Documents)
            .WithOne(e => e.Notification)
            .HasForeignKey(e => e.NotificationId)
            .IsRequired(false);

        modelBuilder.Entity<Notification>()
            .HasMany(e => e.Statuses)
            .WithOne(e => e.Notification)
            .HasForeignKey(e => e.NotificationId);

        modelBuilder.Entity<AdminUser>().HasMany(e => e.Chats).WithOne(e => e.Admin).HasForeignKey(e => e.AdminId);

        modelBuilder.Entity<ActiveStudent>()
            .HasMany(e => e.Chats)
            .WithOne(e => e.ActiveStudent)
            .HasForeignKey(e => e.ActiveStudentId);

        modelBuilder.Entity<Chat>().HasMany(e => e.Messages).WithOne(e => e.Chat).HasForeignKey(e => e.ChatId);

        modelBuilder.Entity<Message>()
            .HasMany(e => e.Documents)
            .WithOne(e => e.Message)
            .HasForeignKey(e => e.MessageId)
            .IsRequired(false);

        modelBuilder.Entity<Message>().HasOne(e => e.Status).WithOne(e => e.Message).HasForeignKey("MessageStatus");
    }
}
