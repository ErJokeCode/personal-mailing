using Core.Models;
using Microsoft.EntityFrameworkCore;
using Notify.Abstractions.Parser;
using Notify.Models;

namespace Notify.Data;

public class AppDbContext : DbContext
{
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Admin> Users => Set<Admin>();
    public DbSet<GroupAssignment> GroupAssignments => Set<GroupAssignment>();
    public DbSet<Core.Models.Chat> Chats => Set<Core.Models.Chat>();
    public DbSet<Message> Messages => Set<Message>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.BuildParserModels();
    }
}