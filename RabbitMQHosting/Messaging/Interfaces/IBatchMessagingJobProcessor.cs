using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitMQHosting.Messaging
{
    public interface IBatchMessagingJobProcessor<in TMessage>
    {
        Task Process(IEnumerable<TMessage> message, CancellationToken cancellationToken);
    }
}
