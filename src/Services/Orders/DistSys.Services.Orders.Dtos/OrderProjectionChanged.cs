namespace DistSys.Services.Orders.Dtos;

public record OrderProjectionChanged(
    Guid OrderId,
    OrderStatus Status,
    DeliveryDetails DeliveryDetails,
    PaymentInformation PaymentInformation,
    List<ProductQuantity> Products,
    int Version
);

public enum OrderStatus
{
    Created,
    Paid,
    Dispatched,
    Completed,
    Failed,
}
