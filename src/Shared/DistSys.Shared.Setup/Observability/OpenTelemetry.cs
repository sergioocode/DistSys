using DistSys.Shared.Discovery;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace DistSys.Shared.Setup.Observability;

public static class OpenTelemetry
{
    private static string? _openTelemetryUrl;

    public static void AddTracing(
        this IServiceCollection serviceCollection,
        IConfiguration configuration
    )
    {
        string appName = GetAppName(configuration);
        serviceCollection.AddOpenTelemetry().WithTracing(builder =>
            builder
                .SetResourceBuilder(
                    ResourceBuilder.CreateDefault().AddService(appName)
                )
                .AddAspNetCoreInstrumentation()
                .AddOtlpExporter(exporter =>
                {
                    string url = GetOpenTelemetryCollectorUrl(
                        serviceCollection.BuildServiceProvider()
                    ).Result;
                    exporter.Endpoint = new Uri(url);
                })
        );
    }

    public static void AddMetrics(
        this IServiceCollection serviceCollection,
        IConfiguration configuration
    )
    {
        string appName = GetAppName(configuration);
        serviceCollection.AddOpenTelemetry().WithMetrics(builder =>
            builder
                .SetResourceBuilder(
                    ResourceBuilder.CreateDefault().AddService(appName)
                )
                .AddAspNetCoreInstrumentation()
                .AddOtlpExporter(exporter =>
                {
                    string url = GetOpenTelemetryCollectorUrl(
                        serviceCollection.BuildServiceProvider()
                    ).Result;
                    exporter.Endpoint = new Uri(url);
                })
        );
    }

    // No se usa en DistSys; se conserva como referencia del artículo.
    public static void AddLogging(this IHostBuilder builder, IConfiguration configuration)
    {
        string appName = GetAppName(configuration);
        builder.ConfigureLogging(logging =>
            logging
                //Next line optional to remove other providers
                .ClearProviders()
                .AddOpenTelemetry(options =>
                {
                    options.IncludeFormattedMessage = true;
                    options.SetResourceBuilder(
                        ResourceBuilder.CreateDefault().AddService(appName)
                    );
                    options.AddConsoleExporter();
                })
        );
    }

    private static async Task<string> GetOpenTelemetryCollectorUrl(IServiceProvider serviceProvider)
    {
        if (_openTelemetryUrl != null)
            return _openTelemetryUrl;

        IServiceDiscovery serviceDiscovery =
            serviceProvider.GetRequiredService<IServiceDiscovery>();
        string openTelemetryLocation = await serviceDiscovery.GetFullAddress(
            DiscoveryServices.OpenTelemetry
        );
        _openTelemetryUrl = $"http://{openTelemetryLocation}";
        return _openTelemetryUrl;
    }

    private static string GetAppName(IConfiguration configuration) =>
        configuration["AppName"]
        ?? throw new InvalidOperationException(
            "No se configuró el nombre de la aplicación para OpenTelemetry."
        );
}
