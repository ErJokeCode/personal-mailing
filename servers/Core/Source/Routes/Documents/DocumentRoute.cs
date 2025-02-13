using System;
using System.Threading.Tasks;
using Core.Infrastructure.Errors;
using Core.Routes.Documents.Commands;
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
        var group = app.MapGroup("/core/documents");

        group.MapGet("/{documentId}", DownloadDocument);
    }

    // TODO Add validator for guid, implement documents on notifications, test it out
    // Damn, with this approch we cant have contentType and fileName since document is owned
    // One possible solution is to store metadata in the blob storage itself as well
    // Or just require to pass the whole document object as json into the download method
    // also setup depends on in the docker compose file, because right now stuff might break
    public async Task<Results<FileStreamHttpResult, NotFound<ProblemDetails>>> DownloadDocument(Guid documentId, IMediator mediator)
    {
        var command = new DownloadDocumentCommand()
        {
            DocumentId = documentId,
        };

        var result = await mediator.Send(command);


        if (result.IsFailed)
        {
            return result.ToNotFoundProblem();
        }

        return TypedResults.File(result.Value);
    }
}