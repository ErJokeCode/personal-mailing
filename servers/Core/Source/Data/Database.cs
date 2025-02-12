using System;
using System.Collections.Generic;
using System.Text.Json;
using Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Core.Data;

public class AppDbContext : IdentityDbContext<Admin, IdentityRole<Guid>, Guid>
{
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Notification> Notifications => Set<Notification>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Student>().OwnsOne(s => s.Info, i =>
        {
            i.OwnsMany(i => i.OnlineCourse, o =>
            {
                o.Property(x => x.Scores).HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
                    v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions)null!)!
                );

            });
        });

        builder.Entity<Student>().OwnsOne(s => s.Info, i =>
        {
            i.OwnsMany(i => i.Subjects, s =>
            {
                s.OwnsOne(s => s.OnlineCourse, o =>
                {
                    o.Property(o => o.Scores).HasConversion(
                        v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
                        v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions)null!)!
                    );
                });
            });
        });
    }
}