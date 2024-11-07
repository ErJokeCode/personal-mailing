using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Models.Dto;

public interface IMappable<TDto, TOrig>
{
    public static abstract TDto Map(TOrig orig);
    public static abstract List<TDto> Maps(List<TOrig> origs);
}

public class AdminUserDto : IMappable<AdminUserDto, AdminUser>
{
    public string Id { get; set; }
    public string Email { get; set; }
    public string Date { get; set; }

    public static AdminUserDto Map(AdminUser orig)
    {
        return new AdminUserDto() { Id = orig.Id, Email = orig.Email, Date = orig.Date };
    }

    public static List<AdminUserDto> Maps(List<AdminUser> origs)
    {
        return origs.Select(o => AdminUserDto.Map(o)).ToList();
    }
}

public class ActiveStudentDto : IMappable<ActiveStudentDto, ActiveStudent>
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string ChatId { get; set; }
    public string AdminChatId { get; set; } = null;
    public string Date { get; set; }

    public List<string> OnboardStatus { get; set; } = [];

    public Student Info { get; set; }

    public static ActiveStudentDto Map(ActiveStudent orig)
    {
        return new ActiveStudentDto() {
            Id = orig.Id,
            Email = orig.Email,
            ChatId = orig.ChatId,
            AdminChatId = orig.AdminChatId,
            Info = orig.Student,
            Date = orig.Date,
            OnboardStatus = orig.OnboardStatus,
        };
    }

    public static List<ActiveStudentDto> Maps(List<ActiveStudent> origs)
    {
        return origs.Select(o => ActiveStudentDto.Map(o)).ToList();
    }
}
