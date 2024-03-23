using System;

namespace Tracker.BuildingBlocks.MessageHub.Kafka
{
    internal static class Constants
    {
        public static TimeSpan ReconnectBackoff = TimeSpan.FromMinutes(10);
        public static TimeSpan MaxPollInterval = TimeSpan.FromMinutes(30);
        public static TimeSpan SessionTimeout = TimeSpan.FromSeconds(90);
        public static TimeSpan HeartbeatInterval = TimeSpan.FromSeconds(5);
        public static TimeSpan MetadataMaxAge = TimeSpan.FromSeconds(100);
    }
}
