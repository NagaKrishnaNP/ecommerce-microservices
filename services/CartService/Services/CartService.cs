using System.Text.Json;
using StackExchange.Redis;
using CartService.Models;

namespace CartService.Services;

public class CartServiceRedis
{
    private readonly IDatabase _db;

    public CartServiceRedis(IConnectionMultiplexer redis)
    {
        _db = redis.GetDatabase();
    }

    public async Task<Cart> GetCart(string userId)
    {
        var data = await _db.StringGetAsync(userId);

        if (data.IsNullOrEmpty)
            return new Cart { UserId = userId };

        return JsonSerializer.Deserialize<Cart>(data!)!;
    }

    public async Task SaveCart(Cart cart)
    {
        var json = JsonSerializer.Serialize(cart);
        await _db.StringSetAsync(cart.UserId, json);
    }

    public async Task ClearCart(string userId)
    {
        await _db.KeyDeleteAsync(userId);
    }
}