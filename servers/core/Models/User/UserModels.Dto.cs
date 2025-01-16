using System;
using System.Collections.Generic;

namespace Core.Models.Dto;

public partial class AdminUserDto
{
    public string Id { get; set; }
    public string Email { get; set; }
    public string Date { get; set; }
    public List<Identity.Permission> Permissions { get; set; } = [];
    public List<string> Groups {get; set;} = [];
}

public partial class ActiveStudentDto
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string ChatId { get; set; }
    public string Date { get; set; }

    public List<string> OnboardStatus { get; set; } = [];

    public Student Info { get; set; }
}
