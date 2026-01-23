using System;
using System.Collections.Generic;
using System.Text;

namespace CleanArchitecture.Application.Filters
{
    public class OrderDetailSearchFilter
    {
        //public string? Keyword { get; set; }
        public int PageSize { get; set; } = 10;
        public int PageNumber { get; set; } = 1;

    }
}
