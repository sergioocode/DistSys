using System.Net;
using DistSys.Services.Orders.BusinessLogic.UseCases;
using DistSys.Services.Orders.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace DistSys.Services.Orders.Controllers;

[ApiController]
[Route("[controller]")]
public class OrderController
{
    private readonly ICreateOrder _createOrder;
    private readonly IChangeOrderStatus _changeOrderStatus;

    public OrderController(ICreateOrder createOrder, IChangeOrderStatus changeOrderStatus)
    {
        _createOrder = createOrder;
        _changeOrderStatus = changeOrderStatus;
    }

    [HttpPost("create")]
    [ProducesResponseType(typeof(ResultDto<CreateOrderResponse>), (int)HttpStatusCode.Created)]
    public async Task<IActionResult> CreateOrder(
        CreateOrderRequest createOrderRequest,
        CancellationToken cancellationToken = default
    )
    {
        CreateOrderResponse response = await _createOrder.Execute(
            createOrderRequest,
            cancellationToken
        );

        return response
            .Success()
            .UseSuccessHttpStatusCode(HttpStatusCode.Created)
            .ToActionResult();
    }

    [HttpPut("markaspaid")]
    [ProducesResponseType(typeof(ResultDto<bool>), (int)HttpStatusCode.Accepted)]
    public Task<IActionResult> OrderPaid(
        Guid orderId,
        CancellationToken cancellationToken = default
    ) => ChangeStatus(orderId, OrderStatus.Paid, cancellationToken);

    [HttpPut("markasdispatched")]
    [ProducesResponseType(typeof(ResultDto<bool>), (int)HttpStatusCode.Accepted)]
    public Task<IActionResult> OrderDispatched(
        Guid orderId,
        CancellationToken cancellationToken = default
    ) => ChangeStatus(orderId, OrderStatus.Dispatched, cancellationToken);

    [HttpPut("markasdelivered")]
    [ProducesResponseType(typeof(ResultDto<bool>), (int)HttpStatusCode.Accepted)]
    public Task<IActionResult> OrderDelivered(
        Guid orderId,
        CancellationToken cancellationToken = default
    ) => ChangeStatus(orderId, OrderStatus.Completed, cancellationToken);

    private async Task<IActionResult> ChangeStatus(
        Guid orderId,
        OrderStatus status,
        CancellationToken cancellationToken
    )
    {
        bool changed = await _changeOrderStatus.Execute(
            orderId,
            status,
            cancellationToken
        );

        return changed
            .Success()
            .UseSuccessHttpStatusCode(HttpStatusCode.Accepted)
            .ToActionResult();
    }
}
