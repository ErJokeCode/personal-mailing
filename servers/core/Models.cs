using Microsoft.EntityFrameworkCore;
using Shared.Core;

namespace Core.Models;

public class CoreDb : DbContext
{
    public DbSet<Student> Students => Set<Student>();

    public CoreDb(DbContextOptions<CoreDb> options) : base(options)
    {
    }
}
