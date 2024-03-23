using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tracker.BuildingBlocks.MessageHub.Abstractions
{
    public interface IHubConsumer
    {
        public void Start<TValue>(string group, string topic, Func<TValue, Task> handler, CancellationToken cancellationToken = default);
        public void Stop();
    }
}