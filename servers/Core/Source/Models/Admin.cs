using System;
using Microsoft.AspNetCore.Identity;

namespace Core.Models;

public class Admin : IdentityUser<Guid>
{
    public DateOnly CreatedAt { get; set; }
}