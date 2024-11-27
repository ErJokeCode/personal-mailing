using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Utility;

public class PaginatedList<T>
{
    public List<T> Items { get; } = [];
    public int TotalPages { get; set; }
    public int PageIndex { get; set; }
    public int PageSize { get; set; }

    public PaginatedList(List<T> items, int pageIndex, int pageSize)
    {
        if (pageSize == -1)
        {
            Items.AddRange(items);
            PageIndex = 0;
            PageSize = items.Count;
            TotalPages = 1;
        }
        else
        {
            var paginated = items.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

            Items.AddRange(paginated);
            PageIndex = pageIndex;
            PageSize = paginated.Count;
            TotalPages = (int)Math.Ceiling(items.Count / (double)pageSize);
        }
    }
}
