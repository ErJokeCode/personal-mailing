using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Riok.Mapperly.Abstractions;

namespace Core.Models;

public class Admin : IdentityUser<Guid>
{
    public required DateOnly CreatedAt { get; set; }

    public ICollection<string> Groups { get; set; } = [];

    [MapperIgnore]
    public ICollection<Notification> Notifications { get; set; } = [];

    [MapperIgnore]
    public override DateTimeOffset? LockoutEnd { get; set; }
    [PersonalData]
    [MapperIgnore]
    public override bool TwoFactorEnabled { get; set; }
    [PersonalData]
    [MapperIgnore]
    public override bool PhoneNumberConfirmed { get; set; }
    [ProtectedPersonalData]
    [MapperIgnore]
    public override string? PhoneNumber { get; set; }
    [MapperIgnore]
    public override string? ConcurrencyStamp { get; set; }
    [MapperIgnore]
    public override string? SecurityStamp { get; set; }
    [MapperIgnore]
    public override string? PasswordHash { get; set; }
    [PersonalData]
    [MapperIgnore]
    public override bool EmailConfirmed { get; set; }
    [MapperIgnore]
    public override string? NormalizedEmail { get; set; }
    [MapperIgnore]
    public override string? NormalizedUserName { get; set; }
    [MapperIgnore]
    public override bool LockoutEnabled { get; set; }
    [MapperIgnore]
    public override int AccessFailedCount { get; set; }
}