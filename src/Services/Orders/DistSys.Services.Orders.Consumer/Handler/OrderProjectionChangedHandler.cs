using DistSys.Services.Orders.BusinessLogic.DataAccess;
using DistSys.Services.Orders.BusinessLogic.Services.External;
using DistSys.Services.Orders.Dtos;
using DistSys.Shared.Communication.Consumer.Handler;
using DistSys.Shared.Communication.Messages;

namespace DistSys.Services.Orders.Consumer.Handler;

public class OrderProjectionChangedHandler
    : IDomainMessageHandler<OrderProjectionChanged>
{
    private readonly IOrdersReadStore _readStore;
    private readonly IProductNameService _productNameService;

    public OrderProjectionChangedHandler(
        IOrdersReadStore readStore,
        IProductNameService productNameService
    )
    {
        _readStore = readStore;
        _productNameService = productNameService;
    }

    public async Task Handle(
        DomainMessage<OrderProjectionChanged> message,
        CancellationToken cancelToken = default
    )
    {
        OrderProjectionChanged order = message.Content;
        List<ProductQuantityName> products = new();

        foreach (ProductQuantity product in order.Products)
        {
            string name = await _productNameService.GetProductName(
                product.ProductId,
                cancelToken
            );
            products.Add(new ProductQuantityName(product.ProductId, product.Quantity, name));
        }

        OrderResponse projection = new(
            order.OrderId,
            order.Status.ToString(),
            order.DeliveryDetails,
            order.PaymentInformation,
            products
        );

        await _readStore.UpsertOrder(projection, order.Version, cancelToken);
    }
}
