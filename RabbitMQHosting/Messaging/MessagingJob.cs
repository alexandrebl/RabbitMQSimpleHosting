using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQSimpleConnectionFactory.Entity;
using RabbitMQSimpleConnectionFactory.Library;
using RabbitMQSimpleConsumer;
using RabbitMQSimpleHosting.Hosting.Interfaces;
using RabbitMQSimpleHosting.Messaging.Interfaces;

namespace RabbitMQSimpleHosting.Messaging
{
    public class MessagingJob<TMessage, TProcessor> : IWorkerJob where TProcessor : IMessagingJobProcessor<TMessage> where TMessage : class
    {
        private static readonly MessagingJobProcessorAttribute _attribute;

        private readonly ILogger _logger;
        private readonly string _queueName = $"{_attribute.Queue}.processing";
        private readonly IServiceProvider _services;
        private readonly ConnectionSetting _connectionSetting;

        static MessagingJob()
        {
            var attr = typeof(TProcessor).GetTypeInfo().GetCustomAttribute<MessagingJobProcessorAttribute>();
            _attribute = attr ?? throw new InvalidOperationException("TProcessor must have a MessagingJobProcessor attribute");
        }

        public MessagingJob(IServiceProvider services, ILogger logger)
        {
            var connectionSetting = services.GetService<ConnectionSetting>();

            _logger = logger;
            _services = services;
            _connectionSetting = connectionSetting;
        }

        public async Task Start(CancellationToken cancellationToken)
        {
            try
            {

                string connectionName = this.GetConnectionName();

                var channelFactory = new ChannelFactory(_connectionSetting, connectionName);

                for (var index = 0; index < _attribute.ConsumerCount; index++)
                {
                    var queueManager = new QueueManager<TMessage>(_queueName).WithChannelFactory(channelFactory)
                        .WithConsumer(prefetchCount: _attribute.Prefetch);

                    var scope = _services.CreateScope();
                    var instance = ActivatorUtilities.CreateInstance<TProcessor>(scope.ServiceProvider);

                    queueManager.Consumer.ReceiveMessage +=
                        (
                            message,
                            deliveryTag) =>
                        {
                            try
                            {
                                instance.ProcessAsync(message, new CancellationToken()).Wait(cancellationToken);
                                queueManager.Consumer.Ack(deliveryTag);
                            }
                            catch (Exception exception)
                            {
                                queueManager.Consumer.Nack(deliveryTag, false);

                                _logger?.LogError(exception, $"Error on receive message for queue {_queueName}");
                            }
                        };

                    queueManager.Consumer.OnReceiveMessageException +=
                        (
                            exception,
                            deliveryTag) =>
                        {
                            queueManager.Consumer.Nack(deliveryTag, false);

                            _logger?.LogError(exception, $"Unable to process message for queue {_queueName}. Error message: {exception.Message}");
                        };

                    queueManager.Consumer.WatchInit();
                }
            }
            catch (Exception exception)
            {
                _logger?.LogError(exception, $"Unable to create ProcessingWorker for queue {_queueName}");
            }
        }

        private string GetConnectionName()
        {
            Type processorType = typeof(TProcessor);
            var assemblyName = processorType.Assembly.GetName();
            string app = assemblyName.Name;
            string version = assemblyName.Version.ToString();

            return $"{app}({version})@{Environment.MachineName}/{this._connectionSetting.VirtualHost}/{processorType.Name}-Consumer";
        }
    }
}