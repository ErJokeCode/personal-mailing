using System;
using System.Collections.Generic;
using Yarp.ReverseProxy.Configuration;
using Yarp.ReverseProxy.Transforms;

namespace Core.Setup.Proxy;

public static class KnowledgeBaseProxy
{
    public static RouteConfig[] GetRoutes()
    {
        return
        [
            new RouteConfig()
            {
                RouteId = "base-route",
                ClusterId = "base-cluster",
                Match = new RouteMatch
                {
                    Path = "/base/{**catch-all}"
                },
            }.WithTransformPathRouteValues("/api/v1/{**catch-all}")
        ];
    }

    public static ClusterConfig[] GetClusters()
    {
        return
        [
            new ClusterConfig()
            {
                ClusterId = "base-cluster",

                Destinations = new Dictionary<string, DestinationConfig>(StringComparer.OrdinalIgnoreCase)
                {
                    {
                        "destination",
                        new DestinationConfig()
                        {
                            Address = "http://base:8080"
                        }
                    },
                }
            }
        ];
    }
}