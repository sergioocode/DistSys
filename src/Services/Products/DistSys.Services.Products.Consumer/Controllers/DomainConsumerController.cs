using DistSys.Shared.Communication.Consumer.Host;
using DistSys.Shared.Communication.Consumer.Manager;
using DistSys.Shared.Communication.Messages;
using Microsoft.AspNetCore.Mvc;

namespace DistSys.Services.Products.Consumer.Controllers;

[ApiController]
[Route("[controller]")]
public class DomainConsumerController : ConsumerController<DomainMessage>
{
    public DomainConsumerController(IConsumerManager<DomainMessage> consumerManager)
        : base(consumerManager) { }
}
