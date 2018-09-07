using System;
using System.IO;
using RabbitMQSimpleHosting.Hosting.Interfaces;

namespace RabbitMQSimpleHosting.Hosting
{
    public class HostingEnvironment : IHostingEnvironment
    {
        public string EnvironmentName { get; set; }
        public string BasePath { get; set; }

        public HostingEnvironment()
        {
            EnvironmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            BasePath = Directory.GetCurrentDirectory();
        }
    }
}
