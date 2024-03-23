using System;

namespace Tracker.Storage.Application.Abstractions
{
    public class UserVisitedEventDto
    {
        public DateTime Timestamp { get; set; }

        public string Referrer { get; set; }

        public string UserAgent { get; set; }

        public string IpAddress { get; set; }
    }
}
