namespace BuildingBlocks.Shared.Contracts.Orders;

public record OrderCreated(
    Guid OrderId,
    Guid CreatedByUserId,
    decimal TotalAmount,
    string Status,
    DateTime CreatedAtUtc
);