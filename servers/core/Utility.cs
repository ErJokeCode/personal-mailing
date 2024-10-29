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
    }
}
