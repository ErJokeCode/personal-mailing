using System;
using Microsoft.EntityFrameworkCore;

namespace Core.Models;

[Owned]
[Keyless]
public class Document
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required string MimeType { get; set; }
}