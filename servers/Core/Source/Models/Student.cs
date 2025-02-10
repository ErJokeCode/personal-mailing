using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Core.External.Parser;

namespace Core.Models;

public class Student
{
    public Guid Id { get; set; }
    public required string Email { get; set; }
    public required string ChatId { get; set; }
    public required DateOnly CreatedAt { get; set; }

    [NotMapped]
    public ParserStudent? Info { get; set; }

    public ICollection<Notification> Notifications { get; } = [];
}
