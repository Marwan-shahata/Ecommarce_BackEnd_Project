using ECommerce.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.BLL
{
    public class OrderSummaryDto
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public OrderStatus Status { get; set; }
        public string StatusDisplay { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public int ItemCount { get; set; }
        public DateTime PlacedAt { get; set; }
    }

}
