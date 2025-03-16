using System.Text.Json.Serialization;

namespace Endurance_Testing.Models
{
    public class TestSummary
    {
        public int TotalRequestsProcessed { get; set; }
        public int TotalSuccessfulRequests { get; set; }
        public int TotalFailedRequests { get; set; }

        [JsonIgnore]
        public int CurrentRound { get; set; }

        [JsonIgnore]
        public double TotalCpuUsage { get; set; }

        [JsonIgnore]
        public double TotalRamUsage { get; set; }

        [JsonIgnore]
        public double TotalLoadTime { get; set; }

        [JsonIgnore]
        public double TotalWaitTime { get; set; }

        [JsonIgnore]
        public double TotalResponseTime { get; set; }

        [JsonIgnore]
        public int TotalResponses { get; set; }

        [JsonIgnore]
        public double TotalThroughput { get; set; }

        public double AverageCpuUsage => CurrentRound > 0 ? TotalCpuUsage / CurrentRound : 0;
        public double AverageRamUsage => CurrentRound > 0 ? TotalRamUsage / CurrentRound : 0;
        public double AverageLoadTime => TotalResponses > 0 ? TotalLoadTime / TotalResponses : 0;
        public double AverageWaitTime => TotalResponses > 0 ? TotalWaitTime / TotalResponses : 0;
        public double AverageResponseTime => TotalResponses > 0 ? TotalResponseTime / TotalResponses : 0;
        public double AverageThroughput => CurrentRound > 0 ? TotalThroughput / CurrentRound : 0;
        public double AverageErrorRate => TotalRequestsProcessed > 0 ? (double)TotalFailedRequests / TotalRequestsProcessed * 100 : 0;
        public double AverageRoundDuration { get; set; }
    }
}