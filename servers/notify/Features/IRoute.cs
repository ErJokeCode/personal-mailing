using Microsoft.AspNetCore.Builder;

namespace Notify.Features;

public interface IRoute
{
    void MapRoutes(WebApplication app);
}