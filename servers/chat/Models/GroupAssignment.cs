using System;
using Notify.Models;
using Riok.Mapperly.Abstractions;

namespace Core.Models;

public class GroupAssignment
{
    public int Id { get; set; }
    public required string Name { get; set; }

    public required Guid AdminId { get; set; }
    [MapperIgnore]
    public Admin? Admin { get; set; }
}