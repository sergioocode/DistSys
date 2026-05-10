using Microsoft.Extensions.Configuration;

namespace Distribt.Shared.Setup.Services;

public static class SecretManager
{
    public static void AddSecretManager(
        this IServiceCollection serviceCollection,
        IConfiguration configuration
    )
    {
        //TODO: create an awaiter project instead of .result everywhere in the config
        string discoveredUrl = GetVaultUrl(serviceCollection.BuildServiceProvider()).Result;
        serviceCollection.AddVaultService(configuration, discoveredUrl);
    }

    private static async Task<string> GetVaultUrl(IServiceProvider serviceProvider)
    {
        IServiceDiscovery serviceDiscovery =
            serviceProvider.GetService<IServiceDiscovery>()
            ?? throw new InvalidOperationException("Service discovery is not registered.");

        string vaultLocation = await serviceDiscovery.GetFullAddress(DiscoveryServices.Secrets);

        return $"http://{vaultLocation}";
    }
}
