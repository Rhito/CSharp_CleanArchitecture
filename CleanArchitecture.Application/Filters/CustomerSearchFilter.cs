using System;
using System.Collections.Generic;
using System.Text;

namespace CleanArchitecture.Application.Filters
{
    public class CustomerSearchFilter
    {
        // Keyword filter for customer name or email
        public string? Keyword { get; set; }
        // Date range filters for customer creation date
        public DateTime? FromCreatedAt { get; set; }
        public DateTime? ToCreatedAt { get; set; }
        // Include deleted customers filter
        public bool? IsDeleted { get; set; }
        // Pagination properties
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
