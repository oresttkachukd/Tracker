using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Confluent.Kafka.DependencyInjection;
using Tracker.BuildingBlocks.MessageHub.Abstractions;

namespace Tracker.BuildingBlocks.MessageHub.Kafka
{
    public static class IoCConfig
    {
        public static IServiceCollection AddKafkaMessageHub(this IServiceCollection services, IConfiguration configuration)
        {
            var settings = configuration.GetRequiredSection("Kafka").Get<KafkaSettings>();
            var servers = JsonSerializer.Deserialize<string[]>(settings.Servers);

            var clientConfig = new ClientConfig
            {
                BootstrapServers = string.Join(',', servers),
                MetadataMaxAgeMs = (int)Constants.MetadataMaxAge.TotalMilliseconds,
                SocketKeepaliveEnable = true
            };

            services.AddSingleton(clientConfig);
            services.AddKafkaClient(clientConfig);
            services.AddSingleton<IHubProducer, KafkaProducer>();
            services.AddSingleton<IHubConsumer, KafkaConsumer>();

            return services;
        }
    }
}
