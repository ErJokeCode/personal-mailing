using System;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Chatter.Routes;
using Chatter.Routes.Documents.Commands;
using Shared.Infrastructure.Errors;

namespace Chatter.Routes.Documents;

public class DocumentRoute : IRoute
{
    public void MapRoutes(WebApplication app)
    {
        var group = app.MapGroup("/core/chats/documents");

        group.MapGet("/{blobId}", DownloadDocument)
            .WithDescription("Скачивает документ по айди");
    }

    public async Task<Results<FileStreamHttpResult, NotFound<ProblemDetails>, ValidationProblem>> DownloadDocument(
        Guid blobId, IMediator mediator
    )
    {
        var command = new DownloadDocumentCommand()
        {
            BlobId = blobId,
        };

        var result = await mediator.Send(command);

        if (result.IsFailed)
        {
            return result.ToNotFoundProblem();
        }

        return TypedResults.File(result.Value.Stream, result.Value.ContentType, result.Value.Name);
    }
}