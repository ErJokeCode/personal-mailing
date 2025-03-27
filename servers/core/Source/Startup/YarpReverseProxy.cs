using System;
using System.Collections.Generic;
using Yarp.ReverseProxy.Configuration;
using Yarp.ReverseProxy.Transforms;

namespace Core;

public static class YarpReverseProxy
{
    public static RouteConfig[] GetRoutes()
    {
        return
        [
            new RouteConfig()
            {
                RouteId = "parser-route",
                ClusterId = "parser-cluster",
                Match = new RouteMatch
                {
                    Path = "/parser/{**catch-all}"
                },
            },
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
                ClusterId = "parser-cluster",

                Destinations = new Dictionary<string, DestinationConfig>(StringComparer.OrdinalIgnoreCase)
                {
                    {
                        "destination",
                        new DestinationConfig()
                        {
                            Address = "http://parser:8000"
                        }
                    },
                }
            },
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