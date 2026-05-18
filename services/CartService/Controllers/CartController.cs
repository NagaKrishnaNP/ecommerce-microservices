using Microsoft.AspNetCore.Mvc;
using CartService.Services;
using CartService.Models;

namespace CartService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CartController : ControllerBase
{
    private readonly CartServiceRedis _cartService;

    public CartController(CartServiceRedis cartService)
    {
        _cartService = cartService;
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetCart(string userId)
    {
        var cart = await _cartService.GetCart(userId);
        return Ok(cart);
    }

    [HttpPost("{userId}/add")]
    public async Task<IActionResult> AddToCart(string userId, CartItem item)
    {
        var cart = await _cartService.GetCart(userId);

        if (cart == null)
        {
            cart = new Cart
            {
                UserId = userId,
                Items = new List<CartItem>()
            };
        }

        var existing = cart.Items.FirstOrDefault(x => x.ProductId == item.ProductId);

        if (existing != null)
            existing.Quantity += item.Quantity;
        else
            cart.Items.Add(item);

        await _cartService.SaveCart(cart);

        return Ok(cart);
    }

    [HttpDelete("{userId}/clear")]
    public async Task<IActionResult> ClearCart(string userId)
    {
        await _cartService.ClearCart(userId);
        return Ok(new { message = "Cart cleared" });
    }

    [HttpDelete("{userId}/remove/{productId}")]
public async Task<IActionResult> RemoveFromCart(string userId, int productId)
{
    var cart = await _cartService.GetCart(userId);

    if (cart == null || !cart.Items.Any())
        return NotFound("Cart is empty");

    var item = cart.Items.FirstOrDefault(x => x.ProductId == productId);

    if (item == null)
        return NotFound("Item not found");

    cart.Items.Remove(item);

    // If cart becomes empty → delete key (clean approach)
    if (!cart.Items.Any())
        await _cartService.ClearCart(userId);
    else
        await _cartService.SaveCart(cart);

    return Ok(cart);
}
}