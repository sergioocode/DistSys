using DistSys.Services.Orders.BusinessLogic.DataAccess;
using DistSys.Services.Orders.BusinessLogic.UseCases;
using DistSys.Shared.Setup.API;
using DistSys.Shared.Setup.Databases;
using DistSys.Shared.Setup.Services;

WebApplication app = DefaultDistSysWebApplication.Create(
    args,
    webappBuilder =>
    {
        webappBuilder
            .Services.AddMySql<OrdersWriteStore>("distribt")
            .AddScoped<IOrdersWriteStore, OrdersWriteStore>()
            .AddScoped<ICreateOrder, CreateOrder>()
            .AddScoped<IChangeOrderStatus, ChangeOrderStatus>()
            .AddServiceBusDomainPublisher(webappBuilder.Configuration);
    }
);

DefaultDistSysWebApplication.Run(app);
