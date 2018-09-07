using System;

namespace RabbitMQSimpleHosting.Messaging
{
    public class MessagingJobProcessorAttribute : Attribute
    {
        public string Queue { get; }
        public TimeSpan ProcessingTimeout { get; }
        public ushort Prefetch { get; }
        public ushort ConsumerCount { get; }

        public MessagingJobProcessorAttribute(string queue, ushort prefetch = 1, ushort consumerCount = 1, double processingTimeoutMilliseconds = 30000)
        {
            Queue = queue;
            ProcessingTimeout = TimeSpan.FromMilliseconds(processingTimeoutMilliseconds);
            Prefetch = prefetch;
            ConsumerCount = consumerCount;
        }
    }
}
