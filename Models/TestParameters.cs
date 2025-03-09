using System;

namespace Endurance_Testing.Models
{
    public class TestParameters
    {
        public string Url { get; set; }

        public int MinRequests { get; set; }

        public int MaxRequests { get; set; }

        public string Mode { get; set; }

        public int TimeoutInSeconds { get; set; }

        public long DurationInSeconds { get; set; }

        public string SelectedTimePeriod { get; set; }
    }
}