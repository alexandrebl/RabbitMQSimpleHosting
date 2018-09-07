using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQSimpleHosting.Hosting.Interfaces;
using RabbitMQSimpleHosting.Jobs.Interfaces;

namespace RabbitMQSimpleHosting.Jobs
{
    public class Job<TProcessor> : IWorkerJob
        where TProcessor : class, IJobProcessor
    {
        private readonly IJobProtector _protector;
        private readonly IServiceProvider _services;

        public Job(IServiceProvider services, IJobProtector protector)
        {
            _protector = protector;
            _services = services;
        }

        public async Task Start(CancellationToken cancellationToken)
        {
            using (_protector.Guard())
            {
                var scope = _services.CreateScope();
                var instance = ActivatorUtilities.CreateInstance<TProcessor>(scope.ServiceProvider);

                await instance.Process(cancellationToken);
            }
        }
    }
}