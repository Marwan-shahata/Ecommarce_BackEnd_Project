using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.BLL
{
    public class PlaceOrderDto
    {
        public string ShippingAddress { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }
}
