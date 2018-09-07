using System.Threading;
using System.Threading.Tasks;

namespace RabbitMQSimpleHosting.Messaging.Interfaces
{
    public interface IMessagingJobProcessor<in TMessage>
    {
        Task Process(TMessage message, CancellationToken cancellationToken);
    }
}
