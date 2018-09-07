namespace RabbitMQSimpleHosting.Hosting.Interfaces
{
    public interface IHostingEnvironment
    {
        string EnvironmentName { get; }
        string BasePath { get; }
    }
}
