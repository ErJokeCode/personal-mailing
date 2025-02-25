using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Infrastructure.Rest;

public class PagedList<T>
{
    public IEnumerable<T> Items { get; set; } = [];
    public required int Page { get; set; }
    public required int PageSize { get; set; }
    public required int TotalCount { get; set; }

    public bool HasNextPage => PageSize != -1 && Page * PageSize < TotalCount;
    public bool HasPreviousPage => Page > 1;

    public static PagedList<T> Create(IEnumerable<T> original, int? page, int? pageSize)
    {
        page ??= 1;
        page = Math.Max(1, (int)page);
        pageSize ??= -1;
        pageSize = Math.Max(-1, (int)pageSize);

        int totalCount = original.Count();
        var items = original;

        if (pageSize != -1)
        {
            items = original.Skip(((int)page - 1) * (int)pageSize).Take((int)pageSize);
        }

        return new PagedList<T>()
        {
            Items = items,
            Page = (int)page,
            PageSize = (int)pageSize,
            TotalCount = totalCount
        };
    }
}