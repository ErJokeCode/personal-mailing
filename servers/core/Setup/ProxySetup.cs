using System;
using System.Collections.Generic;
using Yarp.ReverseProxy.Configuration;
using Yarp.ReverseProxy.Transforms;

namespace Core.Setup;

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
                RouteId = "notify-route",
                ClusterId = "notify-cluster",
                Match = new RouteMatch
                {
                    Path = "/core/notifications/{**catch-all}"
                },
            },
            new RouteConfig()
            {
                RouteId = "chat-route",
                ClusterId = "chat-cluster",
                Match = new RouteMatch
                {
                    Path = "/core/chats/{**catch-all}"
                },
            },
            new RouteConfig()
            {
                RouteId = "group-route",
                ClusterId = "chat-cluster",
                Match = new RouteMatch
                {
                    Path = "/core/groups/{**catch-all}"
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