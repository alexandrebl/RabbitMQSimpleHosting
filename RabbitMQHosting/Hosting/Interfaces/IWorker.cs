using System.Threading.Tasks;

namespace RabbitMQSimpleHosting.Hosting.Interfaces
{
    public interface IWorker
    {
        Task Run();
        Task Stop();
    }
}
