using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.DAL
{
    public interface ICartRepository : IGenericRepository<Cart>
    {
        /// <summary>
        /// Eagerly loads Items → Product so the BLL doesn't need extra queries.
        /// </summary>
        Task<Cart?> GetByUserIdAsync(string userId);
        Task<CartItem?> GetCartItemAsync(int cartId, int productId);
    }
}
