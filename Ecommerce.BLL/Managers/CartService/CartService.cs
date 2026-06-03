using ECommerce.BLL;
using ECommerce.Common;
using ECommerce.DAL;

namespace ECommerce.BLL
{
    public class CartService : ICartService
    {
        private readonly IUnityOfWork _uow;

        public CartService(IUnityOfWork uow) => _uow = uow;

        public async Task<GeneralResult<CartResponseDto>> GetCartAsync(string userId)
        {
            var cart = await _uow.Carts.GetByUserIdAsync(userId);

            if (cart == null)
            {
                return GeneralResult<CartResponseDto>
                    .SuccessResult(EmptyCartDto());
            }

            return GeneralResult<CartResponseDto>
                .SuccessResult(MapToDto(cart));
        }

        public async Task<GeneralResult<CartResponseDto>> AddToCartAsync(string userId, AddToCartDto dto)
        {
            var product = await _uow.Products.GetByIdAsync(dto.ProductId);

            if (product == null)
            {
                return GeneralResult<CartResponseDto>
                    .NotFound($"Product with ID {dto.ProductId} not found.");
            }

            if (product.Stock < dto.Quantity)
            {
                return GeneralResult<CartResponseDto>
                    .Fail($"Only {product.Stock} units in stock.");
            }

            var cart = await _uow.Carts.GetByUserIdAsync(userId);

            if (cart == null)
            {
                cart = new Cart { UserId = userId };

                await _uow.Carts.AddAsync(cart);
                await _uow.SaveChangesAsync();
            }

            var existingItem = await _uow.Carts.GetCartItemAsync(cart.Id, dto.ProductId);

            if (existingItem != null)
            {
                existingItem.Quantity += dto.Quantity;

                if (product.Stock < existingItem.Quantity)
                {
                    return GeneralResult<CartResponseDto>
                        .Fail($"Only {product.Stock} units available in total.");
                }
            }
            else
            {
                cart.Items.Add(new CartItem
                {
                    CartId = cart.Id,
                    ProductId = dto.ProductId,
                    Quantity = dto.Quantity,
                    UnitPrice = product.Price
                });
            }

            await _uow.SaveChangesAsync();

            var updatedCart = await _uow.Carts.GetByUserIdAsync(userId);

            return GeneralResult<CartResponseDto>
                .SuccessResult(MapToDto(updatedCart!), "Item added to cart.");
        }

        public async Task<GeneralResult<CartResponseDto>> UpdateCartItemAsync(string userId, UpdateCartItemDto dto)
        {
            var cart = await _uow.Carts.GetByUserIdAsync(userId);

            if (cart == null)
            {
                return GeneralResult<CartResponseDto>
                    .NotFound("Cart not found.");
            }

            var item = await _uow.Carts.GetCartItemAsync(cart.Id, dto.ProductId);

            if (item == null)
            {
                return GeneralResult<CartResponseDto>
                    .NotFound("Item not in cart.");
            }

            var product = await _uow.Products.GetByIdAsync(dto.ProductId);

            if (product!.Stock < dto.Quantity)
            {
                return GeneralResult<CartResponseDto>
                    .Fail($"Only {product.Stock} units in stock.");
            }

            if (dto.Quantity <= 0)
            {
                cart.Items.Remove(item);
            }
            else
            {
                item.Quantity = dto.Quantity;
            }

            await _uow.SaveChangesAsync();

            var updatedCart = await _uow.Carts.GetByUserIdAsync(userId);

            return GeneralResult<CartResponseDto>
                .SuccessResult(
                    updatedCart != null ? MapToDto(updatedCart) : EmptyCartDto(),
                    "Cart updated."
                );
        }

        public async Task<GeneralResult<bool>> RemoveFromCartAsync(string userId, int productId)
        {
            var cart = await _uow.Carts.GetByUserIdAsync(userId);

            if (cart == null)
            {
                return GeneralResult<bool>
                    .NotFound("Cart not found.");
            }

            var item = await _uow.Carts.GetCartItemAsync(cart.Id, productId);

            if (item == null)
            {
                return GeneralResult<bool>
                    .NotFound("Item not found in cart.");
            }

            cart.Items.Remove(item);

            await _uow.SaveChangesAsync();

            return GeneralResult<bool>
                .SuccessResult(true, "Item removed from cart.");
        }

        // ─────────────────────────────────────────────────────────────
        // Mappers
        // ─────────────────────────────────────────────────────────────

        private static CartResponseDto MapToDto(Cart cart)
        {
            var items = cart.Items.Select(i => new CartItemResponseDto
            {
                ProductId = i.ProductId,
                ProductName = i.Product?.Name ?? string.Empty,
                ProductImageUrl = i.Product?.ImageUrl,
                UnitPrice = i.UnitPrice,
                Quantity = i.Quantity,
                SubTotal = i.UnitPrice * i.Quantity
            }).ToList();

            return new CartResponseDto
            {
                Items = items,
                TotalAmount = (int)items.Sum(i => i.SubTotal),
                TotalItems = items.Sum(i => i.Quantity)
            };
        }

        private static CartResponseDto EmptyCartDto()
        {
            return new CartResponseDto
            {
                Items = new List<CartItemResponseDto>(),
                TotalAmount = 0,
                TotalItems = 0
            };
        }
    }
}