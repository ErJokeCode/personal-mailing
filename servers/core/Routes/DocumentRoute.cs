using Core.Handlers;
using Core.Identity;
using Microsoft.AspNetCore.Builder;

namespace Core.Routes;

public static class DocumentRoute
{
    public static void MapDocumentRoutes(this WebApplication app)
    {
        var group = app.MapGroup("/core/document");

        group.MapGet("/{id}", DocumentHandler.GetDocument).RequireAuthorization(Permissions.ViewPolicy);
        group.MapGet("/{id}/data", DocumentHandler.GetDocumentData).RequireAuthorization(Permissions.ViewPolicy);
    }
}
