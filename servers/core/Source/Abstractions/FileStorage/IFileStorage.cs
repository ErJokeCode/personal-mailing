using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Core.Models;

namespace Core.Abstractions.FileStorage;

public class FileStorageOptions
{
    public required string ContainerName { get; set; }
}

public interface IFileStorage
{
    public Task<Document> UploadAsync(BlobData data, CancellationToken cancellationToken = default);

    public Task<BlobData?> DownloadAsync(Guid fileId, CancellationToken cancellationToken = default);

    public Task DeleteAsync(Guid fileId, CancellationToken cancellationToken = default);
}