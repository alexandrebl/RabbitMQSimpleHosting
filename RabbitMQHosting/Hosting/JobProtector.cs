using System;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQSimpleHosting.Hosting.Interfaces;

namespace RabbitMQSimpleHosting.Hosting
{
    public class JobProtector : IJobProtector
    {
        private long _counter;

        public JobProtector()
        {
            _counter = 0;
        }

        public IDisposable Guard()
        {
            Interlocked.Increment(ref _counter);

            return new Waiter(this);
        }

        private void release()
        {
            Interlocked.Decrement(ref _counter);
        }

        public async Task WaitAll()
        {
            while (_counter > 0)
                await Task.Delay(100);
        }

        private class Waiter : IDisposable
        {
            private readonly JobProtector _protector;

            public Waiter(JobProtector protector)
            {
                _protector = protector;
            }

            public void Dispose()
            {
                _protector.release();
            }
        }
    }
}
