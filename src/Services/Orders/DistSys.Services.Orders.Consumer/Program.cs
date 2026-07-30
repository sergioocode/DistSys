using DistSys.Services.Orders.BusinessLogic;
using DistSys.Services.Orders.BusinessLogic.DataAccess;
using DistSys.Services.Orders.Consumer.Handler;
using DistSys.Shared.Setup.API;
using DistSys.Shared.Setup.Services;

WebApplication app = DefaultDistSysWebApplication.Create(
    args,
    builder =>
    {
        builder.Services.AddProductService(builder.Configuration);
        builder.Services.AddScoped<IOrdersReadStore, OrdersReadStore>();

        builder.Services.AddHandlersInAssembly<OrderProjectionChangedHandler>();
        builder.Services.AddServiceBusDomainConsumer(builder.Configuration);
        builder.Services.AddServiceBusIntegrationConsumer(builder.Configuration);
    }
);

DefaultDistSysWebApplication.Run(app);
