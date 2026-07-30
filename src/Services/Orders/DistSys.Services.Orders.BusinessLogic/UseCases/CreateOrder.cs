using DistSys.Services.Orders.BusinessLogic.DataAccess;
using DistSys.Services.Orders.Dtos;
using DistSys.Shared.Communication.Publisher.Domain;
using DistSys.Shared.Discovery;

namespace DistSys.Services.Orders.BusinessLogic.UseCases;

public interface ICreateOrder
{
    Task<CreateOrderResponse> Execute(
        CreateOrderRequest request,
        CancellationToken cancellationToken = default
    );
}

public class CreateOrder : ICreateOrder
{
    private readonly IOrdersWriteStore _writeStore;
    private readonly IDomainMessagePublisher _publisher;
    private readonly IServiceDiscovery _discovery;

    public CreateOrder(
        IOrdersWriteStore writeStore,
        IDomainMessagePublisher publisher,
        IServiceDiscovery discovery
    )
    {
        _writeStore = writeStore;
        _publisher = publisher;
        _discovery = discovery;
    }

    public async Task<CreateOrderResponse> Execute(
        CreateOrderRequest request,
        CancellationToken cancellationToken = default
    )
    {
        OrderProjectionChanged order = await _writeStore.CreateOrder(
            request,
            cancellationToken
        );

        await _publisher.Publish(
            order,
            routingKey: "order",
            cancellationToken: cancellationToken
        );

        string readApi = await _discovery.GetFullAddress(
            DiscoveryServices.Microservices.OrdersApi.ApiRead
        );

        return new CreateOrderResponse(order.OrderId, $"{readApi}/order/{order.OrderId}");
    }
}
