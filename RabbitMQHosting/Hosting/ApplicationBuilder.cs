using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQHosting.Hosting;
using RabbitMQSimpleHosting.Hosting.Interfaces;

namespace RabbitMQSimpleHosting.Hosting
{
    public class ApplicationBuilder : IApplicationBuilder
    {
        private readonly List<Type> _jobs;

        public IServiceProvider ApplicationServices { get; }

        public ApplicationBuilder(IServiceProvider services)
        {
            _jobs = new List<Type>();

            ApplicationServices = services;
        }

        public void Add(Type jobType)
        {
            _jobs.Add(jobType);
        }

        public IWorkerApplication Build()
        {
            var jobs = new IWorkerJob[_jobs.Count];

            for (int i = 0; i < _jobs.Count; i++)
                jobs[i] = buildJob(_jobs[i]);

            return new WorkerApplication(jobs);
        }

        private IWorkerJob buildJob(Type type)
        {
            return (IWorkerJob)ActivatorUtilities.CreateInstance(ApplicationServices, type);
        }
    }
}
