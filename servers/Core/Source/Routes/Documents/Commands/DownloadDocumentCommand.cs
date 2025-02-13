using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Core.Abstractions;
using Core.Routes.Documents.Errors;
using FluentResults;
using MediatR;

namespace Core.Routes.Documents.Commands;

public class DownloadDocumentCommand : IRequest<Result<Stream>>
{
    public required Guid DocumentId { get; set; }
}

public class DownloadDocumentCommandHandler : IRequestHandler<DownloadDocumentCommand, Result<Stream>>
{
    private readonly IFileStorage _fileStorage;

    public DownloadDocumentCommandHandler(IFileStorage fileStorage)
    {
        _fileStorage = fileStorage;
    }

    public async Task<Result<Stream>> Handle(DownloadDocumentCommand request, CancellationToken cancellationToken)
    {
        var stream = await _fileStorage.DownloadAsync(request.DocumentId, cancellationToken);

        if (stream is null)
        {
            return Result.Fail<Stream>(DocumentErrors.NotFound(request.DocumentId));
        }

        return stream;
    }
}
