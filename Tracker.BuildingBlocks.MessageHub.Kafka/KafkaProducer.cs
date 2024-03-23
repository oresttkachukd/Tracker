using System.Text.Json;
using System.Text.Json.Serialization;
using Confluent.Kafka;
using Tracker.BuildingBlocks.MessageHub.Abstractions;

namespace Tracker.BuildingBlocks.MessageHub.Kafka
{
    internal class KafkaProducer : IHubProducer
    {
        private static readonly JsonSerializerOptions SerializerSettings = new JsonSerializerOptions() 
        { 
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull 
        };

        private readonly IProducer<int, string> _producer;

        public KafkaProducer(ClientConfig clientConfig)
        {
            var producerConfig = new ProducerConfig(clientConfig);

            _producer = new ProducerBuilder<int, string>(producerConfig).Build();
        }

        public void Produce<T>(string topic, T message)
        {
            _producer.Produce(topic, new Message<int, string>()
            {
                Value = JsonSerializer.Serialize(message, SerializerSettings)
            });
        }
    }
}
