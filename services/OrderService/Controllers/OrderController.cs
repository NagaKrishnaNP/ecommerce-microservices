using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderService.DTOs;
using OrderService.Models;
using OrderService.Repositories;
using OrderService.Services;

namespace OrderService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly IOrderRepository _repository;
    private readonly RabbitMQPublisher _publisher;


    public OrderController(IOrderRepository repository, RabbitMQPublisher publisher)
    {
        _repository = repository;
        _publisher = publisher;
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(CreateOrderDto dto)
    {
        var order = new Order
        {
            UserId = dto.UserId,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow,
            Items = dto.Items.Select(i => new OrderItem
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                Price = i.Price
            }).ToList()
        };

        order.TotalAmount = order.Items.Sum(i => i.Price * i.Quantity);

        var created = await _repository.CreateAsync(order);

        await _publisher.PublishAsync("order-created", new
{
    order.Id,
    order.UserId,
    order.TotalAmount
});

        return Ok(new OrderResponseDto
        {
            Id = created.Id,
            UserId = created.UserId,
            TotalAmount = created.TotalAmount,
            Status = created.Status,
            Items = created.Items.Select(i => new OrderItemResponseDto
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                Price = i.Price
            }).ToList()
        });
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var orders = await _repository.GetAllAsync();

        return Ok(orders.Select(o => new OrderResponseDto
        {
            Id = o.Id,
            UserId = o.UserId,
            TotalAmount = o.TotalAmount,
            Status = o.Status,
            Items = o.Items.Select(i => new OrderItemResponseDto
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                Price = i.Price
            }).ToList()
        }));
    }
}