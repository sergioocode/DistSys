using DistSys.Services.Orders.BusinessLogic.DataAccess;
using DistSys.Services.Orders.BusinessLogic.Services.External;
using DistSys.Services.Products.Dtos;
using DistSys.Shared.Communication.Consumer.Handler;
using DistSys.Shared.Communication.Messages;

namespace DistSys.Services.Orders.Consumer.Handler;

public class ProductModifierHandler : IIntegrationMessageHandler<ProductUpdated>
{
    private readonly IProductNameService _productNameService;
    private readonly IOrdersReadStore _ordersReadStore;

    public ProductModifierHandler(
        IProductNameService productNameService,
        IOrdersReadStore ordersReadStore
    )
    {
        _productNameService = productNameService;
        _ordersReadStore = ordersReadStore;
    }

    public async Task Handle(
        IntegrationMessage<ProductUpdated> message,
        CancellationToken cancelToken = default
    )
    {
        await _productNameService.SetProductName(
            message.Content.ProductId,
            message.Content.Details.Name,
            cancelToken
        );
        await _ordersReadStore.UpdateProductName(
            message.Content.ProductId,
            message.Content.Details.Name,
            cancelToken
        );
    }
}
