using System;
using System.Collections.Generic;
using Yarp.ReverseProxy.Configuration;

namespace Core.Setup.Proxy;

public static class NotifyProxy
{
    public static RouteConfig[] GetRoutes()
    {
        return
        [
            new RouteConfig()
            {
                RouteId = "notify-route",
                ClusterId = "notify-cluster",
                Match = new RouteMatch
                {
                    Path = "/notify/{**catch-all}"
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
                ClusterId = "notify-cluster",

                Destinations = new Dictionary<string, DestinationConfig>(StringComparer.OrdinalIgnoreCase)
                {
                    {
                        "destination",
                        new DestinationConfig()
                        {
                            Address = "http://notify:5030"
                        }
                    },
                }
            },
        ];
    }
}