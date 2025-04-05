using Microsoft.AspNetCore.Builder;

namespace Notify.Routes;

public interface IRoute
{
    void MapRoutes(WebApplication app);
}