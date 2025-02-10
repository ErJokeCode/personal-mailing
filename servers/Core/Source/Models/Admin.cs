using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Core.Models;

public class Admin : IdentityUser<Guid>
{
    public required DateOnly CreatedAt { get; set; }

    public ICollection<Notification> Notifications { get; } = [];
}