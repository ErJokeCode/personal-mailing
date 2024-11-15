using System.Collections.Generic;
using System.Linq;

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

public partial class ActiveStudentDto : IMappable<ActiveStudentDto, ActiveStudent>
{
    public static ActiveStudentDto Map(ActiveStudent orig)
    {
        return new ActiveStudentDto()
        {
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
