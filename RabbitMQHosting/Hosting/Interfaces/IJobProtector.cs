using System;
using System.Threading.Tasks;

namespace RabbitMQSimpleHosting.Hosting.Interfaces
{
    public interface IJobProtector
    {
        IDisposable Guard();
        Task WaitAll();
    }
}
