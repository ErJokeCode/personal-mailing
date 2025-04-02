using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Notify.Models;

namespace Notify.Abstractions.FileStorage;

public class FileStorageOptions
{
    public required string BucketName { get; set; }
}

public interface IFileStorage
{
    public Task<Document> UploadAsync(BlobData data, CancellationToken cancellationToken = default);

    public Task<BlobData?> DownloadAsync(Guid fileId, CancellationToken cancellationToken = default);

    public Task DeleteAsync(Guid fileId, CancellationToken cancellationToken = default);
}