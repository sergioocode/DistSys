using DistSys.Services.Orders.BusinessLogic.DataAccess;
using DistSys.Services.Orders.Dtos;
using DistSys.Shared.Setup.API;
using DistSys.Shared.Setup.Databases;

WebApplication app = DefaultDistSysWebApplication.Create(
    args,
    builder =>
    {
        builder
            .Services.AddDistSysMongoDbConnectionProvider(builder.Configuration)
            .AddScoped<IOrdersReadStore, OrdersReadStore>();
    }
);

app.MapGet(
    "order/{orderId:guid}",
    async (
        Guid orderId,
        IOrdersReadStore readStore,
        CancellationToken cancellationToken
    ) =>
    {
        OrderResponse? order = await readStore.GetOrder(orderId, cancellationToken);
        return order is null ? Results.NotFound() : Results.Ok(order);
    }
);

app.MapGet(
    "order/getorderstatus/{orderId:guid}",
    async (
        Guid orderId,
        IOrdersReadStore readStore,
        CancellationToken cancellationToken
    ) =>
    {
        OrderResponse? order = await readStore.GetOrder(orderId, cancellationToken);
        return order is null
            ? Results.NotFound()
            : Results.Ok(new { order.OrderId, order.OrderStatus });
    }
);

DefaultDistSysWebApplication.Run(app);
