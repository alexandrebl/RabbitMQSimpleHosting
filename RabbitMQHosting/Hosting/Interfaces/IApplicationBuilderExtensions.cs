using RabbitMQHosting.Hosting;

namespace RabbitMQSimpleHosting.Hosting.Interfaces
{
    public static class ApplicationBuilderExtensions
    {
        public static void Add<T>(this IApplicationBuilder app) where T : IWorkerJob
        {
            app.Add(typeof(T));
        }
    }
}
