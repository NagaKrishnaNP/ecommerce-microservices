namespace OrderService.DTOs;

public class OrderResponseDto
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public decimal TotalAmount { get; set; }

    public string Status { get; set; } = "";

    public List<OrderItemResponseDto> Items { get; set; } = new();
}