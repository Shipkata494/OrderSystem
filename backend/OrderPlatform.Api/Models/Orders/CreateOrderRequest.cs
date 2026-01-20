namespace OrderPlatform.Api.Models.Orders;

public class CreateOrderRequest
{
    public string CustomerName { get; set; } = default!;
    public decimal TotalAmount { get; set; }
}
