using System;
using System.Collections.Generic;
using Core.Models;
using Microsoft.AspNetCore.Identity;
using Riok.Mapperly.Abstractions;

namespace Notify.Models;

public class Admin
{
    public Guid Id { get; set; }
    public required string UserName { get; set; }
    public required string Email { get; set; }
    public required DateOnly CreatedAt { get; set; }

    [MapperIgnore]
    public ICollection<GroupAssignment> Groups { get; set; } = [];

    [MapperIgnore]
    public ICollection<Core.Models.Chat> Chats { get; set; } = [];
}