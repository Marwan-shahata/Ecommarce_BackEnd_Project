using ECommerce.Common;

namespace ECommerce.BLL
{
    public interface IOrderService
    {
        Task<GeneralResult<OrderResponseDto>> PlaceOrderAsync(string userId, PlaceOrderDto dto);

        Task<GeneralResult<IEnumerable<OrderSummaryDto>>> GetUserOrdersAsync(string userId);

        Task<GeneralResult<OrderResponseDto>> GetOrderByIdAsync(string userId, int orderId);
    }
}