using RabbitMQ.Client;
using System.Text;
using Newtonsoft.Json;

namespace OrderService.Services;

public class RabbitMQPublisher
{
    public async Task PublishAsync(string queueName, object message)
    {
        var factory = new ConnectionFactory()
        {
            HostName = "localhost"
        };

        await using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(queue: queueName,
                                        durable: false,
                                        exclusive: false,
                                        autoDelete: false);

        var json = JsonConvert.SerializeObject(message);
        var body = Encoding.UTF8.GetBytes(json);

        await channel.BasicPublishAsync(exchange: "",
                                        routingKey: queueName,
                                        body: body);
    }
}