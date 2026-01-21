namespace BuildingBlocks.Shared.Contracts.Orders;
public record OrderStatusChanged(
    Guid OrderId,
    string Status,
    DateTime ChangedAtUtc
);
