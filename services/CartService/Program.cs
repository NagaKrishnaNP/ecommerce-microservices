using StackExchange.Redis;
using CartService.Services;
using Microsoft.EntityFrameworkCore;
using ProductService.Data;
using ProductService.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Redis
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var config = builder.Configuration["Redis:ConnectionString"];
    return ConnectionMultiplexer.Connect(config!);
});

builder.Services.AddScoped<CartServiceRedis>();

builder.Services.AddDbContext<ProductDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IProductRepository, ProductRepository>();

builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

app.Run();