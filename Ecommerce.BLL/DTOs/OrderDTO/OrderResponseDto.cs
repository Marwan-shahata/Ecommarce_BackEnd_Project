using ECommerce.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.BLL
{
    public class OrderResponseDto
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public OrderStatus Status { get; set; }
        public string StatusDisplay { get; set; } = string.Empty;

        public IEnumerable<OrderItemResponseDto> Items { get; set; }
            = Enumerable.Empty<OrderItemResponseDto>();

        public decimal TotalAmount { get; set; }

        public string ShippingAddress { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;

        public string? Notes { get; set; }

        public DateTime PlacedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
