using DistSys.Services.Subscriptions.Consumer.Handler;
using DistSys.Shared.Setup.API;
using DistSys.Shared.Setup.Services;

WebApplication app = DefaultDistSysWebApplication.Create(
    args,
    x =>
    {
        x.Services.AddScoped<IDependenciaTest, DependenciaTest>();
        x.Services.AddHandlersInAssembly<SubscriptionHandler>();
        x.Services.AddServiceBusIntegrationConsumer(x.Configuration);
    }
);

DefaultDistSysWebApplication.Run(app);
