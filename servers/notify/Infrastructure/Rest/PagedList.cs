using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Notify.Infrastructure.Rest;

public class PagedList<T>
{
    public IEnumerable<T> Items { get; set; } = [];
    public required int Page { get; set; }
    public required int PageSize { get; set; }
    public required int TotalCount { get; set; }

    public int TotalPages => PageSize == -1 ? 1 : (int)Math.Ceiling((float)TotalCount / PageSize);
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
        else
        {
            page = 1;
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