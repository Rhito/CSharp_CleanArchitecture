using System;
using System.Collections.Generic;
using System.Text;

namespace CleanArchitecture.Application.Filters
{
    public class OrderSearchFilter
    {
        public string? Keyword { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
