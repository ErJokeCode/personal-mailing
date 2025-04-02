using System;
using System.Collections.Generic;
using Notify.Abstractions.Parser;
using Riok.Mapperly.Abstractions;

namespace Notify.Models;

public class Student
{
    public Guid Id { get; set; }
    public required string Email { get; set; }
    public required string ChatId { get; set; }
    public required DateOnly CreatedAt { get; set; }

    public required ParserStudent Info { get; set; }

    public bool Active { get; set; } = true;
    public DateOnly? DeactivatedAt { get; set; }

    [MapperIgnore]
    public ICollection<Notification> Notifications { get; set; } = [];
}
