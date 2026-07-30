using DistSys.Shared.Communication.Consumer.Host;
using DistSys.Shared.Communication.Consumer.Manager;
using DistSys.Shared.Communication.Messages;
using Microsoft.AspNetCore.Mvc;

namespace DistSys.Services.Subscriptions.Consumer.Controllers;

[ApiController]
[Route("[controller]")]
public class IntegrationConsumerController : ConsumerController<IntegrationMessage>
{
    public IntegrationConsumerController(IConsumerManager<IntegrationMessage> consumerManager)
        : base(consumerManager) { }
}
