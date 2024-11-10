using Core.Handlers;
using Core.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Core.Routes;

public static class DocumentRoute
{
    public static void MapDocumentRoutes(this WebApplication app)
    {
        var group = app.MapGroup("/core/document");

        MapGet(group);
    }

    public static void MapGet(RouteGroupBuilder group)
    {
        var getGroup = group.MapGroup("/").AddPermission(Permissions.View);

        getGroup.MapGet("/{id}", DocumentHandler.GetDocument);
        getGroup.MapGet("/{id}/data", DocumentHandler.GetDocumentData);
    }
}
