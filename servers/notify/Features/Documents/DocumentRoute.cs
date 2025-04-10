using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Notify.Features.Documents.Commands;

namespace Notify.Features.Documents;

public class DocumentRoute : IRoute
{
    public void MapRoutes(WebApplication app)
    {
        var group = app.MapGroup("/core/notifications/documents");

        group.MapGet("/{blobId}", DownloadDocumentCommand.Handle)
            .WithDescription("Скачивает документ по айди");
    }
}