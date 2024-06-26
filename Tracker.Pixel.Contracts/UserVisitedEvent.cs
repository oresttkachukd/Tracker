﻿using System;

namespace Tracker.Pixel.Contracts
{
    public class UserVisitedEvent
    {
        public DateTime Timestamp { get; set; }

        public string Referrer { get; set; }

        public string UserAgent { get; set; }

        public string IpAddress { get; set; }
    }
}
