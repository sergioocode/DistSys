using DistSys.Shared.Setup.API;
using DistSys.Shared.Setup.Services;

WebApplication app = DefaultDistSysWebApplication.Create(
    args,
    webappBuilder =>
    {
        webappBuilder.Services.AddServiceBusIntegrationPublisher(webappBuilder.Configuration);
    }
);

DefaultDistSysWebApplication.Run(app);

public partial class Program { }
