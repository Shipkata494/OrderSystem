using System.Text.Json;
using BuildingBlocks.Shared.Contracts.Orders;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using OrderPlatform.Domain.Entities;
using OrderPlatform.Infrastructure.Data;
using StackExchange.Redis;

namespace OrderPlatform.Worker.Consumers;

public class OrderCreatedConsumer : IConsumer<OrderCreated>
{
    private readonly AppDbContext _db;
    private readonly IDatabase _redisDb;
    private readonly int _ttlMinutes;

    public OrderCreatedConsumer(AppDbContext db, IConnectionMultiplexer redis, IConfiguration config)
    {
        _db = db;
        _redisDb = redis.GetDatabase();
        _ttlMinutes = config.GetValue<int>("Redis:OrderTtlMinutes", 120);
    }

    public async Task Consume(ConsumeContext<OrderCreated> context)
    {
        var msg = context.Message;

        var payloadJson = JsonSerializer.Serialize(msg);

        var ev = new OrderEvent
        {
            OrderId = msg.OrderId,
            Type = nameof(OrderCreated),
            Payload = payloadJson,
            CreatedAt = DateTime.UtcNow
        };

        var exists = await _db.OrderEvents
            .AnyAsync(x => x.OrderId == msg.OrderId && x.Type == nameof(OrderCreated), context.CancellationToken);

        if (exists)
        {
            Console.WriteLine($"[OrderCreatedConsumer] duplicate ignored for order:{msg.OrderId}");
            return;
        }

        _db.OrderEvents.Add(ev);
        await _db.SaveChangesAsync(context.CancellationToken);

        var key = $"order:{msg.OrderId}";
        await _redisDb.StringSetAsync(
            key,
            payloadJson,
            expiry: TimeSpan.FromMinutes(_ttlMinutes));

        Console.WriteLine($"[OrderCreatedConsumer] saved event + cached {key} (TTL {_ttlMinutes}m)");
    }
}
