using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQSimpleHosting.Hosting.Interfaces;

namespace RabbitMQSimpleHosting.Hosting {
    public class Worker : IWorker
    {
        private CancellationTokenSource _cancellation;
        private readonly IWorkerApplication _app;
        private readonly IJobProtector _protector;
        private readonly ManualResetEvent _resetEvent;

        public Worker(IServiceProvider services, IWorkerApplication app)
        {
            _resetEvent = new ManualResetEvent(false);
            _app = app;
            services.GetService(typeof(IJobProtector));
            
            _protector = services.GetService<IJobProtector>();
        }

        public async Task Run()
        {
            _cancellation = new CancellationTokenSource();

            Console.CancelKeyPress += async (sender, e) => await Stop();

            var jobStartTasks = _app.Jobs.Select(j => j.Start(_cancellation.Token)).ToArray();

            try
            {
                System.IO.File.AppendAllText("app.log.data", $"Application started");
                Task.WaitAll(jobStartTasks);
            }
            catch (Exception e)
            {
                System.IO.File.AppendAllText("app.log.data", $"Application aborted: {e.StackTrace} - {e.TargetSite} - { e.TargetSite}");
                _resetEvent.Set();
            }
            finally
            {
                System.IO.File.AppendAllText("app.log.data", $"Application end");
            }

            _resetEvent.WaitOne();
        }

        public async Task Stop()
        {
            _resetEvent.Set();
            _cancellation.Cancel();
            await _protector.WaitAll();
        }
    }
}
