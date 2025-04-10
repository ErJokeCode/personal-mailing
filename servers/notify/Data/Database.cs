using Microsoft.EntityFrameworkCore;
using Notify.Models;
using Notify.Services.Parser;

namespace Notify.Data;

public class AppDbContext : DbContext
{
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<Admin> Users => Set<Admin>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.BuildParserModels();
    }
}