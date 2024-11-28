using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Core.Models.Dto;

public partial class AdminUserDto : IMappable<AdminUserDto, AdminUser>
{
    public static AdminUserDto Map(AdminUser orig)
    {
        return new AdminUserDto()
        {
            Id = orig.Id,
            Email = orig.Email,
            Date = orig.Date,
            Permissions = orig.Permissions
        };
    }

    public static List<AdminUserDto> Maps(List<AdminUser> origs)
    {
        return origs.Select(o => AdminUserDto.Map(o)).ToList();
    }
}

public static class AdminUserExtensions
{
    public static void BuildAdminUser(this ModelBuilder builder)
    {
        builder.Entity<AdminUser>().HasMany(e => e.Notifications).WithOne(e => e.Admin).HasForeignKey(e => e.AdminId);

        builder.Entity<AdminUser>().HasMany(e => e.Chats).WithOne(e => e.Admin).HasForeignKey(e => e.AdminId);
    }
}

public partial class ActiveStudentDto : IMappable<ActiveStudentDto, ActiveStudent>
{
    public static ActiveStudentDto Map(ActiveStudent orig)
    {
        return new ActiveStudentDto()
        {
            Id = orig.Id,
            Email = orig.Email,
            ChatId = orig.ChatId,
            Info = orig.Student,
            Date = orig.Date,
            OnboardStatus = orig.OnboardStatus,
            Admin = (orig.Admin == null) ? null : AdminUserDto.Map(orig.Admin),
        };
    }

    public static List<ActiveStudentDto> Maps(List<ActiveStudent> origs)
    {
        return origs.Select(o => ActiveStudentDto.Map(o)).ToList();
    }
}

public static class ActiveStudentExtensions
{
    public static void BuildActiveStudent(this ModelBuilder builder)
    {
        builder.Entity<ActiveStudent>().HasMany(e => e.Notifications).WithMany(e => e.ActiveStudents);

        builder.Entity<ActiveStudent>()
            .HasMany(e => e.Chats)
            .WithOne(e => e.ActiveStudent)
            .HasForeignKey(e => e.ActiveStudentId);
    }
}
