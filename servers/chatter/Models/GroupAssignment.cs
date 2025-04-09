using System;
using Chatter.Models;
using Riok.Mapperly.Abstractions;

namespace Chatter.Models;

public class GroupAssignment
{
    public int Id { get; set; }
    public required string Name { get; set; }

    public required Guid AdminId { get; set; }
    public Admin? Admin { get; set; }
}