namespace Tracker.Pixel.Contracts
{
    public class UserVisitedEvent
    {
        public string Referrer { get; set; }

        public string UserAgent { get; set; }

        public string IpAddress { get; set; }
    }
}
