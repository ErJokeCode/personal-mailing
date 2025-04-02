using System;
using Microsoft.EntityFrameworkCore;

namespace Notify.Models;

[Owned]
public class Document
{
    public required Guid BlobId { get; set; }
    public required string Name { get; set; }
    public required string MimeType { get; set; }
}