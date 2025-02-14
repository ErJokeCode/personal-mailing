using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Core.Abstractions.FileStorage;
using Core.Models;
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

    public async Task<Document> UploadAsync(BlobData data, CancellationToken cancellationToken = default)
    {
        var fileId = Guid.NewGuid();
        var blob = _blobContainer.GetBlobClient(fileId.ToString());

        await blob.UploadAsync(data.Stream, new BlobUploadOptions()
        {
            HttpHeaders = new BlobHttpHeaders()
            {
                ContentType = data.ContentType,
            }
        }, cancellationToken);

        await blob.SetMetadataAsync(new Dictionary<string, string> {
            {"name", data.Name},
        }, cancellationToken: cancellationToken);

        return new Document()
        {
            BlobId = fileId,
            Name = data.Name,
            MimeType = MimeTypes.GetMimeType(data.Name),
        };
    }

    public async Task<BlobData?> DownloadAsync(Guid fileId, CancellationToken cancellationToken = default)
    {
        var blob = _blobContainer.GetBlobClient(fileId.ToString());

        if (!await blob.ExistsAsync(cancellationToken))
        {
            return null;
        }

        var response = await blob.DownloadContentAsync(cancellationToken: cancellationToken);

        return new BlobData()
        {
            Stream = response.Value.Content.ToStream(),
            ContentType = response.Value.Details.ContentType,
            Name = response.Value.Details.Metadata["name"],
        };
    }

    public async Task DeleteAsync(Guid fileId, CancellationToken cancellationToken = default)
    {
        var blob = _blobContainer.GetBlobClient(fileId.ToString());

        await blob.DeleteIfExistsAsync(cancellationToken: cancellationToken);
    }
}