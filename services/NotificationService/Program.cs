using NotificationService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<RabbitMQConsumer>();

var app = builder.Build();

// 🔥 START CONSUMER
var consumer = app.Services.GetRequiredService<RabbitMQConsumer>();
await Task.Run(async () => await consumer.Start());

app.MapGet("/", () => "Notification Service Running");

app.Run();