using System.Collections.Generic;
using System.Linq;
using Core.Identity;
using Core.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Yarp.ReverseProxy.Configuration;

namespace Core.Infrastructure.Metadata;

public static class MetadataCollector
{
    public static IEnumerable<RouteMetadata> DiscoverSource(EndpointDataSource endpointDataSource)
    {
        var routes = new List<RouteMetadata>();

        foreach (var route in endpointDataSource.Endpoints)
        {
            var httpMethod = route.Metadata.GetMetadata<HttpMethodMetadata>()?.HttpMethods.FirstOrDefault();
            var desc = route.Metadata.GetMetadata<IEndpointDescriptionMetadata>()?.Description;
            var routePattern = (route as RouteEndpoint)?.RoutePattern?.RawText;

            var tags = route.Metadata.GetMetadata<ITagsMetadata>()?.Tags ?? [];

            if (!tags.Contains(SecretTokenAuthentication.Tag) && httpMethod != null && routePattern != null)
            {
                routes.Add(new RouteMetadata()
                {
                    Description = desc ?? "",
                    HttpMethod = httpMethod,
                    Path = routePattern,
                });
            }
        }

        return routes;
    }

    public static IEnumerable<RouteMetadata> DiscoverRoutes(this WebApplication app)
    {
        var routes = new List<RouteMetadata>();

        var dataSources = (app as IEndpointRouteBuilder).DataSources;

        foreach (var source in dataSources)
        {
            var endpoints = DiscoverSource(source);

            foreach (var endpoint in endpoints)
            {
                if (endpoint.Path.StartsWith("/core"))
                {
                    routes.Add(endpoint);
                }
            }
        }

        var proxyConfig = app.Services.GetRequiredService<IProxyConfigProvider>();
        var config = proxyConfig.GetConfig();

        foreach (var route in config.Routes)
        {
            foreach (var method in route.Match.Methods!)
            {
                routes.Add(new RouteMetadata()
                {
                    Description = route.Metadata!["Description"],
                    HttpMethod = method.ToUpper(),
                    Path = route.Match.Path!,
                });
            }
        }

        return routes;
    }
}