using OrderService.Models;

namespace OrderService.Repositories;

public interface IOrderRepository
{
    Task<Order> CreateAsync(Order order);

    Task<IEnumerable<Order>> GetAllAsync();
}