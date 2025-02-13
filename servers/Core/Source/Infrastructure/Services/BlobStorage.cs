using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Core.Abstractions;
using Microsoft.Extensions.Options;

namespace Core.Infrastructure.Services;

public class BlobStorage : IFileStorage
{
    private readonly BlobServiceClient _blobService;
    private readonly BlobContainerClient _blobContainer;
    private readonly string _containerName;

    public BlobStorage(BlobServiceClient blobService, IOptions<FileStorageOptions> options)
    {
        _blobService = blobService;
        _containerName = options.Value.ContainerName;
        _blobContainer = _blobService.GetBlobContainerClient(_containerName);

        _blobContainer.CreateIfNotExists();
    }

    public async Task<Guid> UploadAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        var fileId = Guid.NewGuid();
        var blob = _blobContainer.GetBlobClient(fileId.ToString());

        await blob.UploadAsync(stream, cancellationToken);

        return fileId;
    }

    public async Task<Stream?> DownloadAsync(Guid fileId, CancellationToken cancellationToken = default)
    {
        var blob = _blobContainer.GetBlobClient(fileId.ToString());

        if (!blob.Exists())
        {
            return null;
        }

        var response = await blob.DownloadContentAsync(cancellationToken: cancellationToken);

        return response.Value.Content.ToStream();
    }

    public async Task DeleteAsync(Guid fileId, CancellationToken cancellationToken = default)
    {
        var blob = _blobContainer.GetBlobClient(fileId.ToString());

        await blob.DeleteIfExistsAsync(cancellationToken: cancellationToken);
    }
}