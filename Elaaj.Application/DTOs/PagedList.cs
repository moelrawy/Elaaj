using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elaaj.Application.DTOs
{
    public class PagedList<T>
    {
        public List<T> Items { get;private set; }
        public int TotalCount { get;private set; }
        public int PageNumber { get;private set; }
        public int PageSize { get;private set; }
        public int totalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < totalPages;

        public PagedList(List<T> items, int totalCount, int pageNumber, int pageSize)
        {
            Items = items;
            TotalCount = totalCount;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }

    }
}
