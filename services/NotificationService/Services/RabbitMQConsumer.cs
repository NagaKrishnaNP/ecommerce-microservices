using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Newtonsoft.Json;

namespace NotificationService.Services;

public class RabbitMQConsumer
{
    public async Task Start()
    {
        var factory = new ConnectionFactory()
        {
            HostName = "localhost"
        };

        var connection = await factory.CreateConnectionAsync();
        var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(
            queue: "order-created",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            Console.WriteLine("🔥 Order Received:");
            Console.WriteLine(message);

            // simulate notification
            Console.WriteLine("📩 Sending notification to user...");
        };

        await channel.BasicConsumeAsync(
            queue: "order-created",
            autoAck: true,
            consumer: consumer
        );
    }
}