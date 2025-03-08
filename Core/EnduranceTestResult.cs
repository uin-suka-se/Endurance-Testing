using System;
using System.Net;

namespace Endurance_Testing.Core
{
    public class EnduranceTestResult
    {
        public HttpStatusCode StatusCode { get; set; }
        public string ReasonPhrase { get; set; }
        public TimeSpan LoadTime { get; set; }
        public TimeSpan WaitTime { get; set; }
        public TimeSpan ResponseTime { get; set; }
        public int Round { get; set; }
        public double CpuUsage { get; set; }
        public double RamUsage { get; set; }
        public int RequestPerRound { get; set; }
        public int SuccessfulRequests { get; set; }
        public int FailedRequests { get; set; }
        public double AverageLoadTime { get; set; }
        public double AverageWaitTime { get; set; }
        public double AverageResponseTime { get; set; }
        public double Throughput { get; set; }
        public double ErrorRate { get; set; }
        public double RoundDuration { get; set; }
    }
}