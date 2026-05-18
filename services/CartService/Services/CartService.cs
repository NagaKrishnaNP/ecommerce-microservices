using System.Text.Json;
using StackExchange.Redis;
using CartService.Models;

namespace CartService.Services;

public class CartServiceRedis
{
    private readonly IDatabase _db;

    public CartServiceRedis(IConnectionMultiplexer redis)
    {
        _db = redis.GetDatabase(0);
    }

    public async Task<Cart?> GetCart(string userId)
    {
        var data = await _db.StringGetAsync($"cart:{userId}");

        if (string.IsNullOrEmpty(data))
            return new Cart { UserId = userId, Items = new List<CartItem>() };

        return JsonSerializer.Deserialize<Cart>(data);
    }

    public async Task SaveCart(Cart cart)
    {
        var json = JsonSerializer.Serialize(cart);
        await _db.StringSetAsync($"cart:{cart.UserId}", JsonSerializer.Serialize(cart));
    }

    public async Task ClearCart(string userId)
    {
        await _db.KeyDeleteAsync($"cart:{userId}");
    }
}