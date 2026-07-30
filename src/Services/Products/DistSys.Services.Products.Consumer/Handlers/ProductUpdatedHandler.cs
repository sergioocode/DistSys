using DistSys.Services.Products.BusinessLogic.DataAccess;
using DistSys.Services.Products.Dtos;
using DistSys.Shared.Communication.Consumer.Handler;
using DistSys.Shared.Communication.Messages;
using DistSys.Shared.Communication.Publisher.Integration;

namespace DistSys.Services.Products.Consumer.Handlers;

public class ProductUpdatedHandler : IDomainMessageHandler<ProductUpdated>
{
    private readonly IProductsReadStore _readStore;
    private readonly IIntegrationMessagePublisher _integrationMessagePublisher;

    public ProductUpdatedHandler(
        IProductsReadStore readStore,
        IIntegrationMessagePublisher integrationMessagePublisher
    )
    {
        _readStore = readStore;
        _integrationMessagePublisher = integrationMessagePublisher;
    }

    public async Task Handle(
        DomainMessage<ProductUpdated> message,
        CancellationToken cancelToken = default(CancellationToken)
    )
    {
        await _readStore.UpsertProductViewDetails(
            message.Content.ProductId,
            message.Content.Details,
            cancelToken
        );

        await _integrationMessagePublisher.Publish(
            new ProductUpdated(message.Content.ProductId, message.Content.Details),
            routingKey: "external",
            cancellationToken: cancelToken
        );
    }
}
