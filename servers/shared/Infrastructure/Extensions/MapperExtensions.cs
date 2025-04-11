using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Shared.Infrastructure.Extensions;

public interface IMapper { }

public static class MapperExtensions
{
    public static void AddMappers(this IServiceCollection services, Assembly assembly)
    {
        IEnumerable<Type> mappers = assembly
            .GetTypes()
            .Where(t => t.IsAssignableTo(typeof(IMapper)) && !t.IsAbstract && !t.IsInterface);

        foreach (var mapper in mappers)
        {
            services.AddTransient(mapper);
        }
    }
}