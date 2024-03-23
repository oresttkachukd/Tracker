using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Tracker.BuildingBlocks.MessageHub.Abstractions;

namespace Tracker.BuildingBlocks.MessageHub.Kafka
{
    internal class KafkaConsumer : IHubConsumer
    {
        private ClientConfig _clientConfig;
        private IConsumer<Ignore, string> _consumer;        
        private CancellationTokenSource _cancellationTokenSource;

        public KafkaConsumer(ClientConfig clientConfig)
        {
            _clientConfig = clientConfig;
        }

        public void Start<TValue>(string group, string topic, Func<TValue, Task> handler, CancellationToken cancellationToken)
        {
            var consumerConfig = new ConsumerConfig(_clientConfig)
            {
                EnableAutoCommit = false,
                EnableAutoOffsetStore = true,
                GroupId = group,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                ReconnectBackoffMaxMs = (int)Constants.ReconnectBackoff.TotalMilliseconds,
                MaxPollIntervalMs = (int)Constants.MaxPollInterval.TotalMilliseconds,
                SessionTimeoutMs = (int)Constants.SessionTimeout.TotalMilliseconds,
                HeartbeatIntervalMs = (int)Constants.HeartbeatInterval.TotalMilliseconds,
            };

            _consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build();
            _consumer.Subscribe(new[] { topic });

            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            Task.Run(() => StartConsuming(handler, _cancellationTokenSource.Token));
            Task.Run(() => StartCommiting(_cancellationTokenSource.Token));
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel();
        }

        private async Task StartConsuming<TValue>(Func<TValue, Task> handler, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var result = _consumer.Consume(cancellationToken);
                    var message = JsonSerializer.Deserialize<TValue>(result.Message.Value);

                    await handler(message);
                }
                catch
                {
                    await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
                }
            }
        }

        private async Task StartCommiting(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    _consumer.Commit();

                    await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
                }
                catch
                {
                    await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
                }
            }
        }
    }
}
