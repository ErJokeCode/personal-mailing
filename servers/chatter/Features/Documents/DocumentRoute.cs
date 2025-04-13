using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Chatter.Features.Documents.Commands;
using Shared.Infrastructure.Extensions;

namespace Chatter.Features.Documents;

public class DocumentRoute : IRoute
{
    public void MapRoutes(WebApplication app)
    {
        var group = app.MapGroup("/chatter/documents");

        group.MapGet("/{blobId}", DownloadDocumentCommand.Handle)
            .WithDescription("Скачивает документ по айди");
    }
}