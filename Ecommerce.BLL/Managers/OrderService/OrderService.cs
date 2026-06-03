using ECommerce.Common;
using ECommerce.DAL;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.BLL
{
    public class OrderService : IOrderService
    {
        private readonly IUnityOfWork _uow;

        public OrderService(IUnityOfWork uow) => _uow = uow;

        public async Task<GeneralResult<OrderResponseDto>> PlaceOrderAsync(string userId, PlaceOrderDto dto)
        {
            OrderResponseDto? orderDto = null;

            await _uow.ExecuteInTransactionAsync(async () =>
            {
                var cart = await _uow.Carts.GetByUserIdAsync(userId);

                if (cart == null || !cart.Items.Any())
                    throw new InvalidOperationException("Cannot place an order with an empty cart.");

                // Validate stock
                foreach (var item in cart.Items)
                {
                    var product = await _uow.Products.GetByIdAsync(item.ProductId);

                    if (product == null || product.Stock < item.Quantity)
                        throw new InvalidOperationException(
                            $"Insufficient stock for product '{product?.Name ?? item.ProductId.ToString()}'."
                        );
                }

                var orderNumber = await _uow.Orders.GenerateOrderNumberAsync();

                var order = new Order
                {
                    UserId = userId,
                    OrderNumber = orderNumber,
                    Status = OrderStatus.Pending,
                    ShippingAddress = dto.ShippingAddress,
                    City = dto.City,
                    PostalCode = dto.PostalCode,
                    Country = dto.Country,
                    Notes = dto.Notes
                };

                // Create order items + reduce stock
                foreach (var item in cart.Items)
                {
                    var product = await _uow.Products.GetByIdAsync(item.ProductId);

                    order.Items.Add(new OrderItem
                    {
                        ProductId = item.ProductId,
                        UnitPrice = item.UnitPrice,
                        Quantity = item.Quantity
                    });

                    product!.Stock -= item.Quantity;

                    _uow.Products.Update(product);
                }

                order.TotalAmount = order.Items.Sum(i => i.UnitPrice * i.Quantity);

                await _uow.Orders.AddAsync(order);

                _uow.Carts.Remove(cart);

                await _uow.SaveChangesAsync();

                var fullOrder = await _uow.Orders.GetOrderWithItemsAsync(order.Id, userId);

                orderDto = MapToFullDto(fullOrder!);
            });

            return GeneralResult<OrderResponseDto>
                .SuccessResult(orderDto!, "Order placed successfully.");
        }

        public async Task<GeneralResult<IEnumerable<OrderSummaryDto>>> GetUserOrdersAsync(string userId)
        {
            var orders = await _uow.Orders.GetUserOrdersAsync(userId);

            var dtos = orders.Select(MapToSummaryDto);

            return GeneralResult<IEnumerable<OrderSummaryDto>>
                .SuccessResult(dtos);
        }

        public async Task<GeneralResult<OrderResponseDto>> GetOrderByIdAsync(string userId, int orderId)
        {
            var order = await _uow.Orders.GetOrderWithItemsAsync(orderId, userId);

            if (order == null)
            {
                return GeneralResult<OrderResponseDto>
                    .NotFound($"Order with ID {orderId} not found.");
            }

            return GeneralResult<OrderResponseDto>
                .SuccessResult(MapToFullDto(order));
        }

        // ─────────────────────────────────────────────────────────────
        // Mappers
        // ─────────────────────────────────────────────────────────────

        private static OrderResponseDto MapToFullDto(Order o)
        {
            return new OrderResponseDto
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                Status = o.Status,
                StatusDisplay = o.Status.ToString(),
                Items = o.Items.Select(i => new OrderItemResponseDto
                {
                    ProductId = i.ProductId,
                    ProductName = i.Product?.Name ?? string.Empty,
                    ProductImageUrl = i.Product?.ImageUrl,
                    UnitPrice = i.UnitPrice,
                    Quantity = i.Quantity,
                    SubTotal = i.UnitPrice * i.Quantity
                }).ToList(),
                TotalAmount = o.TotalAmount,
                ShippingAddress = o.ShippingAddress,
                City = o.City,
                PostalCode = o.PostalCode,
                Country = o.Country,
                Notes = o.Notes,
                PlacedAt = o.CreatedAt,
                UpdatedAt = o.UpdatedAt
            };
        }

        private static OrderSummaryDto MapToSummaryDto(Order o)
        {
            return new OrderSummaryDto
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                Status = o.Status,
                StatusDisplay = o.Status.ToString(),
                TotalAmount = o.TotalAmount,
                ItemCount = o.Items?.Sum(i => i.Quantity) ?? 0,
                PlacedAt = o.CreatedAt
            };
        }
    }
}