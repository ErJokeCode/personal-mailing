using System;
using System.Collections.Generic;
using Yarp.ReverseProxy.Configuration;

namespace Core;

public static class YarpReverseProxy
{
    public static RouteConfig[] GetRoutes()
    {
        return
        [
            new RouteConfig()
            {
                RouteId = "upload-route",
                ClusterId = "parser-cluster",
                Metadata = new Dictionary<string, string>()
                {
                    {"Description", "Загружает файл со студентами"}
                },
                Match = new RouteMatch
                {
                    Methods = ["Post", "Get"],
                    Path = "/parser/upload/{**catch-all}"
                },
            },
            new RouteConfig()
            {
                RouteId = "parser-route",
                ClusterId = "parser-cluster",
                Metadata = new Dictionary<string, string>()
                {
                    {"Description", "Перенаправляет в парсер"}
                },
                Match = new RouteMatch
                {
                    Methods = ["Get"],
                    Path = "/parser/{**catch-all}"
                },
            }
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
        }
        ];
    }
}