using Core.Models;
using Core.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Core.Utility;

[Authorize]
public class SignalHub : Hub
{
}

public class CoreDb : IdentityDbContext<AdminUser>
{
    public DbSet<ActiveStudent> ActiveStudents => Set<ActiveStudent>();

    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<NotificationTemplate> NotificationTemplates => Set<NotificationTemplate>();
    public DbSet<NotificationStatus> NotificationStatuses => Set<NotificationStatus>();
    public DbSet<NotificationSchedule> NotificationSchedules => Set<NotificationSchedule>();
    public DbSet<Document> Documents => Set<Document>();

    public DbSet<Chat> Chats => Set<Chat>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<MessageStatus> MessageStatuses => Set<MessageStatus>();

    public DbSet<Text> Texts => Set<Text>();

    public CoreDb(DbContextOptions<CoreDb> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.BuildActiveStudent();
        modelBuilder.BuildAdminUser();
        modelBuilder.BuildNotification();
        modelBuilder.BuildChat();
    }
}
