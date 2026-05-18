using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

public class RabbitMQService
{
    private readonly IModel _channel;

    public RabbitMQService()
    {
        var factory = new ConnectionFactory()
        {
            HostName = "localhost"
        };

        var connection = factory.CreateConnection(); // ✅ FIXED
        _channel = connection.CreateModel();

        _channel.QueueDeclare(
            queue: "orderQueue",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );
    }

    public void Publish(object message)
    {
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

        _channel.BasicPublish(
            exchange: "",
            routingKey: "orderQueue",
            basicProperties: null,
            body: body
        );
    }
}