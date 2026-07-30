using DistSys.Shared.Setup.API.Key;
using DistSys.Shared.Setup.API.RateLimiting;

WebApplication app = DefaultDistSysWebApplication.Create(
    args,
    webappBuilder =>
    {
        webappBuilder
            .Services.AddReverseProxy()
            .LoadFromConfig(webappBuilder.Configuration.GetSection("ReverseProxy"));

        webappBuilder.Services.AddApiToken(webappBuilder.Configuration);
        webappBuilder.Services.AddRateLimiter(_ => { });
    }
);

app.UseApiTokenMiddleware();
app.UseRateLimiter();
app.MapGet("/", () => "Hello World!");
app.MapGet(
        "/rate-limiting-test",
        () =>
        {
            return "Hello World!";
        }
    )
    .RequireRateLimiting(new DistSysRateLimiterPolicy());

app.MapReverseProxy();

DefaultDistSysWebApplication.Run(app);
