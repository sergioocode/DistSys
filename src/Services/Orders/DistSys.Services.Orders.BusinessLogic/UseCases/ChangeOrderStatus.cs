using DistSys.Services.Orders.BusinessLogic.DataAccess;
using DistSys.Services.Orders.Dtos;
using DistSys.Shared.Communication.Publisher.Domain;

namespace DistSys.Services.Orders.BusinessLogic.UseCases;

public interface IChangeOrderStatus
{
    Task<bool> Execute(
        Guid orderId,
        OrderStatus status,
        CancellationToken cancellationToken = default
    );
}

public class ChangeOrderStatus : IChangeOrderStatus
{
    private readonly IOrdersWriteStore _writeStore;
    private readonly IDomainMessagePublisher _publisher;

    public ChangeOrderStatus(
        IOrdersWriteStore writeStore,
        IDomainMessagePublisher publisher
    )
    {
        _writeStore = writeStore;
        _publisher = publisher;
    }

    public async Task<bool> Execute(
        Guid orderId,
        OrderStatus status,
        CancellationToken cancellationToken = default
    )
    {
        OrderProjectionChanged? order = await _writeStore.ChangeStatus(
            orderId,
            status,
            cancellationToken
        );

        if (order is null)
            return false;

        await _publisher.Publish(
            order,
            routingKey: "order",
            cancellationToken: cancellationToken
        );
        return true;
    }
}
