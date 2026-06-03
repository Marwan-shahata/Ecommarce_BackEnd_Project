using ECommerce.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.BLL
{
    public interface ICartService
    {
        Task<GeneralResult<CartResponseDto>> GetCartAsync(string userId);

        Task<GeneralResult<CartResponseDto>> AddToCartAsync(string userId, AddToCartDto dto);

        Task<GeneralResult<CartResponseDto>> UpdateCartItemAsync(string userId, UpdateCartItemDto dto);

        Task<GeneralResult<bool>> RemoveFromCartAsync(string userId, int productId);
    }
}
