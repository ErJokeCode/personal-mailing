using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Shared.Models;

namespace Core.Models;

public class CoreDb : IdentityDbContext<AdminUser>
{
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Notification> Notifications => Set<Notification>();

    public CoreDb(DbContextOptions<CoreDb> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Student>()
            .HasMany(e => e.Notifications)
            .WithOne(e => e.Student)
            .HasForeignKey(e => e.StudentId);
    }
}
