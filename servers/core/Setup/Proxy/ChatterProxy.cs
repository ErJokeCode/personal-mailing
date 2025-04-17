using System;
using System.Collections.Generic;
using Yarp.ReverseProxy.Configuration;

namespace Core.Setup.Proxy;

public static class ChatterProxy
{
    public static RouteConfig[] GetRoutes()
    {
        return
        [
            new RouteConfig()
            {
                RouteId = "chat-route",
                ClusterId = "chat-cluster",
                Match = new RouteMatch
                {
                    Path = "/chatter/{**catch-all}"
                },
            },
            new RouteConfig()
            {
                RouteId = "chat-hub-route",
                ClusterId = "chat-cluster",
                Match = new RouteMatch
                {
                    Path = "/chat-hub/{**catch-all}"
                },
            },
        ];
    }

    public static ClusterConfig[] GetClusters()
    {
        return
        [
            new ClusterConfig
            {
                ClusterId = "chat-cluster",

                Destinations = new Dictionary<string, DestinationConfig>(StringComparer.OrdinalIgnoreCase)
                {
                    {
                        "destination",
                        new DestinationConfig()
                        {
                            Address = "http://chatter:5040"
                        }
                    },
                }
            },
        ];
    }
}