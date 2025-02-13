using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Abstractions;

public class FileStorageOptions
{
    public required string ContainerName { get; set; }
}

public interface IFileStorage
{
    public Task<Guid> UploadAsync(Stream stream, CancellationToken cancellationToken = default);

    public Task<Stream?> DownloadAsync(Guid fileId, CancellationToken cancellationToken = default);

    public Task DeleteAsync(Guid fileId, CancellationToken cancellationToken = default);
}