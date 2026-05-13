using StackExchange.Redis;
using CartService.Services;

var builder = WebApplication.CreateBuilder(args);

// Redis
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var config = builder.Configuration["Redis:ConnectionString"];
    return ConnectionMultiplexer.Connect(config!);
});

builder.Services.AddScoped<CartServiceRedis>();

builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

app.Run();