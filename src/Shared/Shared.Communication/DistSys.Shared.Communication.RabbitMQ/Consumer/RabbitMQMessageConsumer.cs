using DistSys.Shared.Communication.Consumer;
using DistSys.Shared.Communication.Consumer.Handler;
using DistSys.Shared.Communication.Messages;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using ISerializer = DistSys.Shared.Serialization.ISerializer;

namespace DistSys.Shared.Communication.RabbitMQ.Consumer;

public class RabbitMQMessageConsumer<TMessage> : IMessageConsumer<TMessage>
{
    private readonly ISerializer _serializer;
    private readonly RabbitMQSettings _settings;
    private readonly ConnectionFactory _connectionFactory;
    private readonly IHandleMessage _handleMessage;
    private IConnection? _connection;
    private IModel? _channel;

    public RabbitMQMessageConsumer(
        ISerializer serializer,
        IOptions<RabbitMQSettings> settings,
        IHandleMessage handleMessage
    )
    {
        _settings = settings.Value;
        _serializer = serializer;
        _handleMessage = handleMessage;
        _connectionFactory = new ConnectionFactory
        {
            HostName = _settings.Hostname,
            Password = _settings.Credentials!.password,
            UserName = _settings.Credentials.username,
        };
    }

    public async Task StartAsync(CancellationToken cancelToken = default)
    {
        _connection = _connectionFactory.CreateConnection();
        _channel = _connection.CreateModel();

        RabbitMQMessageReceiver receiver = new(
            _channel,
            _serializer,
            _handleMessage
        );

        _channel.BasicConsume(GetCorrectQueue(), false, receiver);

        try
        {
            await Task.Delay(Timeout.Infinite, cancelToken);
        }
        finally
        {
            _channel.Dispose();
            _connection.Dispose();
            _channel = null;
            _connection = null;
        }
    }

    private string GetCorrectQueue()
    {
        return (
                typeof(TMessage) == typeof(IntegrationMessage)
                    ? _settings.Consumer?.IntegrationQueue
                    : _settings.Consumer?.DomainQueue
            ) ?? throw new ArgumentException("please configure the queues on the appsettings");
    }
}
