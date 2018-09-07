using System;
using RabbitMQSimpleHosting.Hosting.Interfaces;

namespace RabbitMQHosting.Hosting
{
    public interface IApplicationBuilder
    {
        IServiceProvider ApplicationServices { get; }
        void Add(Type jobType);
        IWorkerApplication Build();
    }
}
