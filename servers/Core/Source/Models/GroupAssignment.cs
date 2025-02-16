using System;
using Riok.Mapperly.Abstractions;

namespace Core.Models;

public class GroupAssignment
{
    public int Id { get; set; }
    public required string Name { get; set; }

    [MapperIgnore]
    public required Guid AdminId { get; set; }
    public Admin? Admin { get; set; }
}