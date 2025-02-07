using Microsoft.AspNetCore.Builder;

namespace Core.Routes.Students;

public class StudentRoute : IRoute
{
    public void MapRoutes(WebApplication app)
    {
        var group = app.MapGroup("/student").RequireAuthorization();
    }
}