using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Notify.Features.Documents.Commands;
using Shared.Infrastructure.Extensions;

namespace Notify.Features.Documents;

public class DocumentRoute : IRoute
{
    public void MapRoutes(WebApplication app)
    {
        var group = app.MapGroup("/notify/documents")
            .WithTags("Документы");

        group.MapGet("/{blobId}", DownloadDocumentCommand.Handle)
            .WithDescription("Скачивает документ по айди");
    }
}