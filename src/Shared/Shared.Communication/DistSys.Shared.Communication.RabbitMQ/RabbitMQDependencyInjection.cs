using DistSys.Shared.Communication.Consumer;
using DistSys.Shared.Communication.Consumer.Handler;
using DistSys.Shared.Communication.Messages;
using DistSys.Shared.Communication.Publisher;
using DistSys.Shared.Communication.RabbitMQ;
using DistSys.Shared.Communication.RabbitMQ.Consumer;
using DistSys.Shared.Communication.RabbitMQ.Publisher;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace DistSys.Shared.Communication.RabbitMQ;

public static class RabbitMQDependencyInjection
{
    public static void AddRabbitMQ(
        this IServiceCollection serviceCollection,
        Func<IServiceProvider, Task<RabbitMQCredentials>> rabbitMqCredentialsFactory,
        Func<IServiceProvider, Task<string>> rabbitMqHostName,
        IConfiguration configuration,
        string name
    )
    {
        serviceCollection.AddRabbitMQ(configuration);
        serviceCollection.PostConfigure<RabbitMQSettings>(x =>
        {
            ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
            x.SetCredentials(rabbitMqCredentialsFactory.Invoke(serviceProvider).Result);
            x.SetHostName(rabbitMqHostName.Invoke(serviceProvider).Result);
        });

        serviceCollection.AddSingleton<IConnectionFactory>(AddRabbitMqHealthCheck);
        serviceCollection
            .AddHealthChecks()
            .AddRabbitMQ(name: name, failureStatus: HealthStatus.Unhealthy);
    }

    private static IConnectionFactory AddRabbitMqHealthCheck(IServiceProvider serviceProvider)
    {
        RabbitMQSettings settings = serviceProvider
            .GetRequiredService<IOptions<RabbitMQSettings>>()
            .Value;
        ConnectionFactory factory = new ConnectionFactory();
        factory.UserName = settings.Credentials?.username;
        factory.Password = settings.Credentials?.password;
        factory.VirtualHost = "/";
        factory.HostName = settings.Hostname;
        factory.Port = AmqpTcpEndpoint.UseDefaultPort;
        return factory;
    }

    /// <summary>
    /// this method is used when the credentials are inside the configuration. not recommended.
    /// </summary>
    public static void AddRabbitMQ(
        this IServiceCollection serviceCollection,
        IConfiguration configuration
    )
    {
        serviceCollection.Configure<RabbitMQSettings>(configuration.GetSection("Bus:RabbitMQ"));
    }

    public static void AddConsumerHandlers(
        this IServiceCollection serviceCollection,
        IEnumerable<IMessageHandler> handlers
    )
    {
        serviceCollection.AddSingleton<IMessageHandlerRegistry>(
            new MessageHandlerRegistry(handlers)
        );
        serviceCollection.AddSingleton<IHandleMessage, HandleMessage>();
    }

    public static void AddRabbitMqConsumer<TMessage>(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddConsumer<TMessage>();
        serviceCollection.AddSingleton<
            IMessageConsumer<TMessage>,
            RabbitMQMessageConsumer<TMessage>
        >();
    }

    public static void AddRabbitMQPublisher<TMessage>(this IServiceCollection serviceCollection)
        where TMessage : IMessage
    {
        serviceCollection.AddPublisher<TMessage>();
        serviceCollection.AddSingleton<
            IExternalMessagePublisher<TMessage>,
            RabbitMQMessagePublisher<TMessage>
        >();
    }
}
