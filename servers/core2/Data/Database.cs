using System;
using Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Core.Data;

public class CoreDb : IdentityDbContext<Admin, IdentityRole<Guid>, Guid>
{
    public CoreDb(DbContextOptions<CoreDb> options) : base(options)
    {
    }
}