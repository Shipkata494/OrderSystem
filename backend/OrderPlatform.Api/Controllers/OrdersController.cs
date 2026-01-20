using System.Security.Claims;
using BuildingBlocks.Shared.Contracts.Orders;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderPlatform.Api.Models.Orders;
using OrderPlatform.Domain.Entities;
using OrderPlatform.Infrastructure.Data;
using StackExchange.Redis;

namespace OrderPlatform.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IDatabase _redisDb;
    private readonly IPublishEndpoint _publish;

    public OrdersController(AppDbContext db, IPublishEndpoint publish, IConnectionMultiplexer redis)
    {
        _db = db;
        _publish = publish;
        _redisDb = redis.GetDatabase();
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create(CreateOrderRequest request)
    {
        var userIdStr = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdStr, out var userId))
            return Unauthorized("Invalid user id in token.");

        if (string.IsNullOrWhiteSpace(request.CustomerName))
            return BadRequest("CustomerName is required.");

        if (request.TotalAmount <= 0)
            return BadRequest("TotalAmount must be > 0.");

        var order = new OrderPlatform.Domain.Entities.Order
        {
            Id = Guid.NewGuid(),
            CustomerName = request.CustomerName.Trim(),
            TotalAmount = request.TotalAmount,
            Status = "PendingPayment",
            CreatedAt = DateTime.UtcNow,
            CreatedByUserId = userId
        };

        _db.Orders.Add(order);
        await _db.SaveChangesAsync();

        await _publish.Publish(new OrderCreated(
            order.Id,
            order.CreatedByUserId,
            order.TotalAmount,
            order.Status,
            order.CreatedAt
        ));

        return Ok(new { orderId = order.Id });
    }

    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Get(Guid id)
    {
        var order = await _db.Orders.FirstOrDefaultAsync(x => x.Id == id);
        return order is null ? NotFound() : Ok(order);
    }

    [HttpGet("{id:guid}/cache")]
    [Authorize]
    public async Task<IActionResult> GetCached(Guid id)
    {
        var key = $"order:{id}";
        var value = await _redisDb.StringGetAsync(key);
        if (value.IsNullOrEmpty) return NotFound(new { key, message = "Cache miss" });

        return Ok(new { key, value = value.ToString() });
    }
}
