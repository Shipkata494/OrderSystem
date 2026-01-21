using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace OrderPlatform.Api.Hubs;

[Authorize]
public class OrderHub : Hub
{
    public Task JoinOrder(Guid orderId)
        => Groups.AddToGroupAsync(Context.ConnectionId, OrderGroup(orderId));

    public Task LeaveOrder(Guid orderId)
        => Groups.RemoveFromGroupAsync(Context.ConnectionId, OrderGroup(orderId));

    public static string OrderGroup(Guid orderId) => $"order:{orderId}";
}
