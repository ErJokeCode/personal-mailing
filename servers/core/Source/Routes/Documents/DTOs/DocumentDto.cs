using System;

namespace Core.Routes.Documents.DTOs;

public class DocumentDto
{
    public required Guid BlobId { get; set; }
    public required string Name { get; set; }
    public required string MimeType { get; set; }
}