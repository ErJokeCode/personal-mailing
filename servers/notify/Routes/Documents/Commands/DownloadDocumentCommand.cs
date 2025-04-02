using System;
using System.IO;
using System.Reflection.Metadata;
using System.Threading;
using System.Threading.Tasks;
using Core.Routes.Documents.Errors;
using FluentResults;
using MediatR;
using Notify.Abstractions.FileStorage;

namespace Core.Routes.Documents.Commands;

public class DownloadDocumentCommand : IRequest<Result<BlobData>>
{
    public required Guid BlobId { get; set; }
}

public class DownloadDocumentCommandHandler : IRequestHandler<DownloadDocumentCommand, Result<BlobData>>
{
    private readonly IFileStorage _fileStorage;

    public DownloadDocumentCommandHandler(IFileStorage fileStorage)
    {
        _fileStorage = fileStorage;
    }

    public async Task<Result<BlobData>> Handle(DownloadDocumentCommand request, CancellationToken cancellationToken)
    {
        var data = await _fileStorage.DownloadAsync(request.BlobId, cancellationToken);

        if (data is null)
        {
            return Result.Fail<BlobData>(DocumentErrors.NotFound(request.BlobId));
        }

        return data;
    }
}
