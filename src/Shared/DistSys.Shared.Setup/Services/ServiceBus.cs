using Distribt.Shared.Communication.RabbitMQ;
using Microsoft.Extensions.Configuration;
using VaultSharp.V1.SecretsEngines;

namespace Distribt.Shared.Setup.Services;

public static class ServiceBus
{
    public static void AddServiceBusIntegrationPublisher(
        this IServiceCollection serviceCollection,
        IConfiguration configuration
    )
    {
        serviceCollection.AddRabbitMQ(
            GetRabbitMqSecretCredentials,
            GetRabbitMQHostName,
            configuration,
            "IntegrationPublisher"
        );
        serviceCollection.AddRabbitMQPublisher<IntegrationMessage>();
    }

    /// <summary>
    /// default option (KeyValue) to get credentials using Vault
    /// </summary>
    /// <param name="serviceProvider"></param>
    /// <returns></returns>
    private static async Task<RabbitMQCredentials> GetRabbitMqSecretCredentials(
        IServiceProvider serviceProvider
    )
    {
        ISecretManager? secretManager = serviceProvider.GetService<ISecretManager>();
        return await secretManager!.Get<RabbitMQCredentials>("rabbitmq");
    }

    /// <summary>
    /// this option is used to show the usage of different engines on Vault
    /// </summary>
    private static async Task<RabbitMQCredentials> GetRabbitMqSecretCredentialsfromRabbitMQEngine(
        IServiceProvider serviceProvider
    )
    {
        ISecretManager? secretManager = serviceProvider.GetService<ISecretManager>();
        UsernamePasswordCredentials credentials = await secretManager!.GetRabbitMQCredentials(
            "distribt-role"
        );
        return new RabbitMQCredentials()
        {
            password = credentials.Password,
            username = credentials.Username,
        };
    }

    public static void AddServiceBusIntegrationConsumer(
        this IServiceCollection serviceCollection,
        IConfiguration configuration
    )
    {
        serviceCollection.AddRabbitMQ(
            GetRabbitMqSecretCredentials,
            GetRabbitMQHostName,
            configuration,
            "IntegrationConsumer"
        );
        serviceCollection.AddRabbitMqConsumer<IntegrationMessage>();
    }

    public static void AddServiceBusDomainPublisher(
        this IServiceCollection serviceCollection,
        IConfiguration configuration
    )
    {
        serviceCollection.AddRabbitMQ(
            GetRabbitMqSecretCredentials,
            GetRabbitMQHostName,
            configuration,
            "DomainPublisher"
        );
        serviceCollection.AddRabbitMQPublisher<DomainMessage>();
    }

    public static void AddServiceBusDomainConsumer(
        this IServiceCollection serviceCollection,
        IConfiguration configuration
    )
    {
        serviceCollection.AddRabbitMQ(
            GetRabbitMqSecretCredentials,
            GetRabbitMQHostName,
            configuration,
            "DomainConsumer"
        );
        serviceCollection.AddRabbitMqConsumer<DomainMessage>();
    }

    public static void AddHandlersInAssembly<T>(this IServiceCollection serviceCollection)
    {
        serviceCollection.Scan(scan =>
            scan.FromAssemblyOf<T>()
                .AddClasses(classes => classes.AssignableTo<IMessageHandler>())
                .AsImplementedInterfaces()
                .WithTransientLifetime()
        );

        ServiceProvider sp = serviceCollection.BuildServiceProvider();
        IEnumerable<IMessageHandler> listHandlers = sp.GetServices<IMessageHandler>();
        serviceCollection.AddConsumerHandlers(listHandlers);
    }

    private static async Task<string> GetRabbitMQHostName(IServiceProvider serviceProvider)
    {
        IServiceDiscovery serviceDiscovery =
            serviceProvider.GetService<IServiceDiscovery>()
            ?? throw new InvalidOperationException("Service discovery is not registered.");

        DiscoveryData rabbitMqData = await serviceDiscovery.GetDiscoveryData(
            DiscoveryServices.RabbitMQ
        );

        return rabbitMqData.Server;
    }
}
