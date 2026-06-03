using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.BLL
{ public class CartResponseDto
    {


        public IEnumerable<CartItemResponseDto> Items { get; set; }
        public int TotalAmount { get; set; }

        public int TotalItems { get; set; }


    }
}
