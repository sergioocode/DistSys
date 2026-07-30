using DistSys.Services.Products.BusinessLogic.DataAccess;
using DistSys.Services.Products.Consumer.Handlers;
using DistSys.Shared.Setup.API;
using DistSys.Shared.Setup.Databases;
using DistSys.Shared.Setup.Services;

WebApplication app = DefaultDistSysWebApplication.Create(
    args,
    builder =>
    {
        builder
            .Services.AddDistSysMongoDbConnectionProvider(builder.Configuration)
            .AddScoped<IProductsReadStore, ProductsReadStore>();
        builder.Services.AddServiceBusIntegrationPublisher(builder.Configuration);
        builder.Services.AddHandlersInAssembly<ProductUpdatedHandler>();
        builder.Services.AddServiceBusDomainConsumer(builder.Configuration);
    }
);

DefaultDistSysWebApplication.Run(app);
