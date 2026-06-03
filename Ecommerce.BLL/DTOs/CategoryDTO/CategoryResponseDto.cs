using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.BLL
{
    public class CategoryResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public int ProductCount { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}