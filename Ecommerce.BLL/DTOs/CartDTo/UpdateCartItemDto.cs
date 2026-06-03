using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.BLL
{
    public class UpdateCartItemDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
