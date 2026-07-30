namespace DistSys.Services.Orders.Dtos;

public record OrderResponse(
    Guid OrderId,
    string OrderStatus,
    DeliveryDetails DeliveryDetails,
    PaymentInformation PaymentInformation,
    List<ProductQuantityName> Products
);

public record ProductQuantityName(int ProductId, int Quantity, string Name);
