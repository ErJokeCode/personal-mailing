using Chatter.Models;
using Microsoft.EntityFrameworkCore;
using Chatter.Abstractions.Parser;
using Chatter.Models;

namespace Chatter.Data;

public class AppDbContext : DbContext
{
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Admin> Users => Set<Admin>();
    public DbSet<GroupAssignment> GroupAssignments => Set<GroupAssignment>();
    public DbSet<Chatter.Models.Chat> Chats => Set<Chatter.Models.Chat>();
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