using System.Threading;
using System.Threading.Tasks;

namespace RabbitMQSimpleHosting.Jobs.Interfaces
{
    public interface IJobProcessor
    {
        Task Process(CancellationToken cancellationToken);
    }
}