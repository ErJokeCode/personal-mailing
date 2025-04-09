using System;
using System.Collections.Generic;
using Chatter.Models;
using Microsoft.AspNetCore.Identity;
using Riok.Mapperly.Abstractions;

namespace Chatter.Models;

public class Admin
{
    public Guid Id { get; set; }
    public required string UserName { get; set; }
    public required string Email { get; set; }
    public required DateOnly CreatedAt { get; set; }

    [MapperIgnore]
    public ICollection<GroupAssignment> Groups { get; set; } = [];

    [MapperIgnore]
    public ICollection<Chatter.Models.Chat> Chats { get; set; } = [];
}