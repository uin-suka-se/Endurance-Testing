namespace Endurance_Testing.Models
{
    public class TestSummary
    {
        public int TotalRequestsProcessed { get; set; }
        public int TotalSuccessfulRequests { get; set; }
        public int TotalFailedRequests { get; set; }
        public int CurrentRound { get; set; }
        public double TotalCpuUsage { get; set; }
        public double TotalRamUsage { get; set; }
        public double TotalLoadTime { get; set; }
        public double TotalWaitTime { get; set; }
        public double TotalResponseTime { get; set; }
        public int TotalResponses { get; set; }
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