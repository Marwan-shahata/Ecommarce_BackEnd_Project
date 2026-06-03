using ECommerce.BLL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers;

[Authorize]
public class CartController : BaseApiController
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    [HttpGet]
    public async Task<IActionResult> GetCart()
    {
        var userId = GetCurrentUserId();
        var result = await _cartService.GetCartAsync(userId);
        return HandleResult(result);
    }

    [HttpPost("add")]
    public async Task<IActionResult> Add(AddToCartDto dto)
    {
        var userId = GetCurrentUserId();
        var result = await _cartService.AddToCartAsync(userId, dto);
        return HandleResult(result);
    }

    [HttpPut("update")]
    public async Task<IActionResult> Update(UpdateCartItemDto dto)
    {
        var userId = GetCurrentUserId();
        var result = await _cartService.UpdateCartItemAsync(userId, dto);
        return HandleResult(result);
    }

    [HttpDelete("{productId:int}")]
    public async Task<IActionResult> Remove(int productId)
    {
        var userId = GetCurrentUserId();
        var result = await _cartService.RemoveFromCartAsync(userId, productId);
        return HandleResult(result);
    }
}