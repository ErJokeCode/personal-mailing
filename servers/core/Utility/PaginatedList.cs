using System.Collections.Generic;
using System.Linq;

namespace Core.Utility;

public static class PaginatedList
{
    public static List<T> Create<T>(List<T> source, int pageIndex, int pageSize)
    {
        if (pageSize == -1)
        {
            return source;
        }

        var count = source.Count;
        var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        return new List<T>(items);
    }
}
