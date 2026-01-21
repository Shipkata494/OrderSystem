using BuildingBlocks.Shared.Contracts.Orders;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using OrderPlatform.Api.Hubs;

namespace OrderPlatform.Api.Consumers;

public class OrderStatusChangedConsumer : IConsumer<OrderStatusChanged>
{
    private readonly IHubContext<OrderHub> _hub;

    public OrderStatusChangedConsumer(IHubContext<OrderHub> hub)
    {
        _hub = hub;
    }

    public async Task Consume(ConsumeContext<OrderStatusChanged> context)
    {
        var msg = context.Message;

        await _hub.Clients
            .Group(OrderHub.OrderGroup(msg.OrderId))
            .SendAsync("orderStatusChanged", new
            {
                orderId = msg.OrderId,
                status = msg.Status,
                changedAtUtc = msg.ChangedAtUtc
            });
    }
}
