using System.IO;

namespace Notify.Abstractions.FileStorage;

public class BlobData
{
    public required Stream Stream { get; set; }
    public required string ContentType { get; set; }
    public required string Name { get; set; }
}