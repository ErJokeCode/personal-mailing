using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Shared.Models;

namespace Core.Models;

public class CoreDb : IdentityDbContext<AdminUser>
{
    public DbSet<ActiveStudent> Students => Set<ActiveStudent>();
    public DbSet<Notification> Notifications => Set<Notification>();

    public CoreDb(DbContextOptions<CoreDb> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ActiveStudent>()
            .HasMany(e => e.Notifications)
            .WithOne(e => e.Student)
            .HasForeignKey(e => e.StudentId);
    }
}
