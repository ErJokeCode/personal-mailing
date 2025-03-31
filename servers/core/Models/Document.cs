using System;
using Microsoft.EntityFrameworkCore;

namespace Core.Models;

[Owned]
public class Document
{
    public required Guid BlobId { get; set; }
    public required string Name { get; set; }
    public required string MimeType { get; set; }
}