using DistSys.Services.Subscriptions.Dtos;
using DistSys.Shared.Communication.Publisher.Integration;
using DistSys.Shared.Setup.API;
using DistSys.Shared.Setup.Services;

WebApplication app = DefaultDistSysWebApplication.Create(
    args,
    webappBuilder =>
    {
        webappBuilder
            .Services.AddReverseProxy()
            .LoadFromConfig(webappBuilder.Configuration.GetSection("ReverseProxy"));
        webappBuilder.Services.AddServiceBusIntegrationPublisher(webappBuilder.Configuration);
    }
);

app.MapGet("/", () => "Hello World!");

app.MapPost(
    "/subscribe",
    async (SubscriptionDto subscriptionDto) =>
    {
        IIntegrationMessagePublisher publisher =
            app.Services.GetRequiredService<IIntegrationMessagePublisher>();
        await publisher.Publish(subscriptionDto, routingKey: "subscription");
    }
);

app.MapReverseProxy();

DefaultDistSysWebApplication.Run(app);
