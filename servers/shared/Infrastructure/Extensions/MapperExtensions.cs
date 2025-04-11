using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Shared.Infrastructure.Extensions;

public interface IMapper { }

public static class MapperExtensions
{
    public static void AddMappers(this IServiceCollection services)
    {
        IEnumerable<Type> mappers = typeof(Program).Assembly
            .GetTypes()
            .Where(t => t.IsAssignableTo(typeof(IMapper)) && !t.IsAbstract && !t.IsInterface);

        foreach (var mapper in mappers)
        {
            services.AddTransient(mapper);
        }
    }
}