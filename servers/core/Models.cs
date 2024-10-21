using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Shared.Core;

namespace Core.Models;

public class CoreDb : IdentityDbContext<ApplicationUser>
{
    public DbSet<Student> Students => Set<Student>();

    public CoreDb(DbContextOptions<CoreDb> options) : base(options)
    {
    }
}

public class ApplicationUser : IdentityUser
{
}
