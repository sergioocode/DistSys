using Consul;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DistSys.Shared.Discovery;

public static class DiscoveryDependencyInjection
{
    public static IServiceCollection AddDiscovery(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        return services
            .AddSingleton<IConsulClient, ConsulClient>(provider => new ConsulClient(consulConfig =>
            {
                string address =
                    configuration["Discovery:Address"]
                    ?? throw new InvalidOperationException(
                        "No se configuró la dirección de descubrimiento de Consul."
                    );
                consulConfig.Address = new Uri(address);
            }))
            .AddSingleton<IServiceDiscovery, ConsulServiceDiscovery>();
    }
}
