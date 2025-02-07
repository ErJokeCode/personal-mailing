using Microsoft.AspNetCore.Builder;

namespace Core.Routes;

public interface IRoute
{
    void MapRoutes(WebApplication app);
}