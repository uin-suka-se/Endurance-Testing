using Endurance_Testing.Models;
using System;

namespace Endurance_Testing.Core
{
    public class RoundCompletedEventArgs : EventArgs
    {
        public double CpuUsage { get; }
        public double RamUsage { get; }
        public int SuccessfulRequests { get; }
        public int FailedRequests { get; }
        public double AverageLoadTime { get; }
        public double AverageWaitTime { get; }
        public double AverageResponseTime { get; }
        public double Throughput { get; }
        public double ErrorRate { get; }
        public double RoundDuration { get; }
        public int RequestCount { get; }

        public RoundCompletedEventArgs(
            double cpuUsage, double ramUsage,
            int successfulRequests, int failedRequests,
            double averageLoadTime, double averageWaitTime, double averageResponseTime,
            double throughput, double errorRate, double roundDuration, int requestCount)
        {
            CpuUsage = cpuUsage;
            RamUsage = ramUsage;
            SuccessfulRequests = successfulRequests;
            FailedRequests = failedRequests;
            AverageLoadTime = averageLoadTime;
            AverageWaitTime = averageWaitTime;
            AverageResponseTime = averageResponseTime;
            Throughput = throughput;
            ErrorRate = errorRate;
            RoundDuration = roundDuration;
            RequestCount = requestCount;
        }
    }

    public class ResultReceivedEventArgs : EventArgs
    {
        public EnduranceTestResult Result { get; }
        public int Round { get; }

        public ResultReceivedEventArgs(EnduranceTestResult result, int round)
        {
            Result = result;
            Round = round;
        }
    }
}