namespace RabbitMQSimpleHosting.Hosting.Interfaces
{
    public interface IWorkerApplication
    {
        IWorkerJob[] Jobs { get; }
    }
}
