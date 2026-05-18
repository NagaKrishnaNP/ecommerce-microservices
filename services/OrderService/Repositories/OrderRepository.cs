using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.Models;

namespace OrderService.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly OrderDbContext _context;
    private readonly RabbitMQService _rabbit;

    public OrderRepository(OrderDbContext context, RabbitMQService rabbit)
    {
        _context = context;
        _rabbit = rabbit;
    }

    public async Task<Order> CreateAsync(Order order)
    {
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
        return order;
    }

    public async Task<IEnumerable<Order>> GetAllAsync()
    {
        return await _context.Orders
            .Include(o => o.Items)
            .ToListAsync();
    }

    public async Task PlaceOrder(Order order)
    {
        // 1️⃣ Save to DB
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        foreach (var item in order.Items)
        {
            item.OrderId = order.Id;
        }

        await _context.SaveChangesAsync();

        // 2️⃣ Publish event
        _rabbit.Publish(new
        {
            order.Id,
            order.UserId,
            order.Items
        });
    }
}