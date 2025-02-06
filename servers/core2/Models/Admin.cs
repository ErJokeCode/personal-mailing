using System;
using Microsoft.AspNetCore.Identity;

namespace Core.Models;

public class Admin : IdentityUser<Guid>
{
    public DateOnly RegisterDate { get; set; }
}