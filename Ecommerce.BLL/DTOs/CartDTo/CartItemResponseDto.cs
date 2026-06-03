using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.BLL
{
    public class CartItemResponseDto
    {

        public int ProductId { get; set; }
        public  string ProductName { get; set; }
        public string? ProductImageUrl { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal SubTotal { get; set; }
   
    
    }

}
