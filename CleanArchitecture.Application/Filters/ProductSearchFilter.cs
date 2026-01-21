using System;
using System.Collections.Generic;
using System.Text;

namespace CleanArchitecture.Application.Filters
{
    public class ProductSearchFilter
    {
        // Keyword filter for product name or description
        public string? Keyword { get; set; }
        // Category filter
        public int? CategoryId { get; set; }

        // Price range filters
        public decimal? FromPrice { get; set; }
        public decimal? ToPrice { get; set; }

        // Include deleted products filter
        public bool? IsDeleted { get; set; }

        // Pagination properties
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
