using DistSys.Services.Subscriptions.Dtos;
using DistSys.Shared.Communication.Consumer.Handler;
using DistSys.Shared.Communication.Messages;

namespace DistSys.Services.Subscriptions.Consumer.Handler;

public class UnsubscriptionHandler : IIntegrationMessageHandler<UnsubscriptionDto>
{
    public Task Handle(
        IntegrationMessage<UnsubscriptionDto> message,
        CancellationToken cancelToken = default(CancellationToken)
    )
    {
        Console.WriteLine($"the email {message.Content.Email} has unsubscribed.");
        //TODO: Full use case
        return Task.CompletedTask;
    }
}
