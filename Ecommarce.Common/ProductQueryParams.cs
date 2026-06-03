using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Common
{
    public class ProductQueryParams
    {
        public int? CategoryId { get; set; }
        public string? Name { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SortBy { get; set; } = "createdAt";
        public bool SortDescending { get; set; } = true;
    }
}
