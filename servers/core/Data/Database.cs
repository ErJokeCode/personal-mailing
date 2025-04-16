using System;
using Core.Identity;
using Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Core.Data;

public class AppDbContext : IdentityDbContext<Admin, IdentityRole<Guid>, Guid>
{
    public DbSet<AdminAuthTicket> AdminAuthTickets => Set<AdminAuthTicket>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
}