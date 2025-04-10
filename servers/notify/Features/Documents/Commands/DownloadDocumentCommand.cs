using System;
using System.Threading.Tasks;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Shared.Context.Documents;
using Shared.Infrastructure.Errors;
using Shared.Services.FileStorage;

namespace Notify.Features.Documents.Commands;

public static class DownloadDocumentCommand
{
    public static async Task<Results<FileStreamHttpResult, NotFound<ProblemDetails>, ValidationProblem>> Handle(
        Guid blobId, IFileStorage fileStorage
    )
    {
        var data = await fileStorage.DownloadAsync(blobId);

        if (data is null)
        {
            return Result.Fail<BlobData>(DocumentErrors.NotFound(blobId)).ToNotFoundProblem();
        }

        return TypedResults.File(data.Stream, data.ContentType, data.Name);
    }
}
