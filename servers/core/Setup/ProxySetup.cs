using System;
using System.Collections.Generic;
using Core.Setup.Proxy;
using Yarp.ReverseProxy.Configuration;
using Yarp.ReverseProxy.Transforms;

namespace Core.Setup;

public static class ProxySetup
{
    public static RouteConfig[] GetRoutes()
    {
        return
        [
            ..ParserProxy.GetRoutes(),
            ..NotifyProxy.GetRoutes(),
            ..KnowledgeBaseProxy.GetRoutes(),
            ..ChatterProxy.GetRoutes(),
        ];
    }

    public static ClusterConfig[] GetClusters()
    {
        return
        [
            ..ParserProxy.GetClusters(),
            ..NotifyProxy.GetClusters(),
            ..KnowledgeBaseProxy.GetClusters(),
            ..ChatterProxy.GetClusters(),
        ];
    }
}