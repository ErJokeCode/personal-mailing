using Microsoft.AspNetCore.Builder;

namespace Chatter.Routes;

public interface IRoute
{
    void MapRoutes(WebApplication app);
}