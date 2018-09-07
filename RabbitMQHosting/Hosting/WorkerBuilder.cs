using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQHosting.Hosting;
using RabbitMQSimpleHosting.Hosting.Interfaces;

namespace RabbitMQSimpleHosting.Hosting
{
    public class WorkerBuilder
    {
        private HostingEnvironment _environment;
        private Action<IServiceCollection> _configureServices;
        private Action<IApplicationBuilder> _configure;

        public WorkerBuilder()
        {
            _environment = new HostingEnvironment();
        }

        public WorkerBuilder UseEnvironment(string environment)
        {
            _environment.EnvironmentName = environment;

            return this;
        }

        public WorkerBuilder UseBasePath(string basePath)
        {
            _environment.BasePath = basePath;

            return this;
        }

        public WorkerBuilder UseStartup<T>()
        {
            return UseStartup(typeof(T));
        }

        public WorkerBuilder UseStartup(Type type)
        {
            object instance = null;
            var configureMethod = findConfigureMethod(type);
            var configureServicesMethod = findConfigureServicesMethod(type);

            if (configureMethod != null || configureServicesMethod != null)
            {
                if (!configureMethod.IsStatic || !configureServicesMethod.IsStatic)
                    instance = Activator.CreateInstance(type, _environment);
            }

            if (configureMethod != null)
            {
                _configure = app =>
                {
                    var parameters = configureMethod.GetParameters();
                    var args = new object[parameters.Length];

                    for (int i = 0; i < parameters.Length; i++)
                    {
                        var parameter = parameters[i];

                        if (parameter.ParameterType.IsAssignableFrom(typeof(IApplicationBuilder)))
                        {
                            args[i] = app;
                        }
                        else
                        {
                            args[i] = app.ApplicationServices.GetService(parameter.ParameterType);
                        }
                    }

                    configureMethod.Invoke(instance, args);
                };
            }

            if (configureServicesMethod != null)
                _configureServices = services => configureServicesMethod.Invoke(instance, new[] { services });

            return this;
        }

        public WorkerBuilder ConfigureServices(Action<IServiceCollection> configureServices)
        {
            _configureServices = configureServices;

            return this;
        }

        public WorkerBuilder Configure(Action<IApplicationBuilder> configure)
        {
            _configure = configure;

            return this;
        }

        public IWorker Build()
        {
            var servicesCollection = new ServiceCollection();

            servicesCollection.AddLogging();
            servicesCollection.AddSingleton<IJobProtector, JobProtector>();

            _configureServices?.Invoke(servicesCollection);

            var services = servicesCollection.BuildServiceProvider();
            var app = new ApplicationBuilder(services);

            _configure?.Invoke(app);

            return new Worker(services, app.Build());
        }

        private MethodInfo findConfigureMethod(Type type)
        {
            return findMethod(type, "Configure");
        }

        private MethodInfo findConfigureServicesMethod(Type type)
        {
            var method = findMethod(type, "ConfigureServices");

            if (method == null)
                return null;

            var parameters = method.GetParameters();

            if (parameters.Length != 1)
                throw new InvalidOperationException("ConfigureServices must take exactly one parameter");

            if (!parameters[0].ParameterType.IsAssignableFrom(typeof(IServiceCollection)))
                throw new InvalidOperationException("ConfigureServices parameter must be assignable from IServiceCollection");

            return method;
        }

        private MethodInfo findMethod(Type type, string name)
        {
            return type
                .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
                .Where(x => x.Name == name)
                .FirstOrDefault();
        }
    }
}
