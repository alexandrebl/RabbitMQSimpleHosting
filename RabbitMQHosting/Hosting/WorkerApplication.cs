using System.Collections.Generic;
using System.Linq;
using RabbitMQSimpleHosting.Hosting.Interfaces;

namespace RabbitMQSimpleHosting.Hosting
{
    public class WorkerApplication : IWorkerApplication
    {
        public IWorkerJob[] Jobs { get; }

        public WorkerApplication(IEnumerable<IWorkerJob> jobs)
        {
            Jobs = jobs.ToArray();
        }
    }
}
