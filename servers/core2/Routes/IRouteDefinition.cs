using Microsoft.AspNetCore.Builder;

namespace Core.Routes;

public interface IRouteDefinition
{
    void RegisterRoutes(WebApplication app);
}