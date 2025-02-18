using System;
using System.Threading.Tasks;
using Core.Infrastructure.Errors;
using Core.Routes.Documents.Commands;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Core.Routes.Documents;

public class DocumentRoute : IRoute
{
    public void MapRoutes(WebApplication app)
    {
        var group = app.MapGroup("/core/documents")
            .RequireAuthorization();

        group.MapGet("/{blobId}", DownloadDocument)
            .WithDescription("Скачивает документ по айди");
    }

    // TODO setup depends on in the docker compose file, because right now stuff might break
    public async Task<Results<FileStreamHttpResult, NotFound<ProblemDetails>, ValidationProblem>> DownloadDocument(
        Guid blobId, IMediator mediator, IValidator<DownloadDocumentCommand> validator
    )
    {
        var command = new DownloadDocumentCommand()
        {
            BlobId = blobId,
        };

        var validationResult = await validator.ValidateAsync(command);

        if (!validationResult.IsValid)
        {
            return validationResult.ToValidationProblem();
        }

        var result = await mediator.Send(command);

        if (result.IsFailed)
        {
            return result.ToNotFoundProblem();
        }

        return TypedResults.File(result.Value.Stream, result.Value.ContentType, result.Value.Name);
    }
}