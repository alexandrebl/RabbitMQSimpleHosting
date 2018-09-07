using System.Threading;
using System.Threading.Tasks;

namespace RabbitMQSimpleHosting.Hosting.Interfaces
{
    public interface IWorkerJob
    {
        Task Start(CancellationToken cancellationToken);
    }
}
