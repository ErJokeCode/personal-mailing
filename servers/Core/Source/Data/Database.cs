using System;
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
}