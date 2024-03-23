namespace Tracker.BuildingBlocks.MessageHub.Abstractions
{
    public interface IHubProducer
    {
        void Produce<T>(string topic, T message);
    }
}