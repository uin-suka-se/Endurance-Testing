using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Endurance_Testing.Models;

namespace Endurance_Testing.Core
{
    public class TestRunner
    {
        private List<EnduranceTestResult> _testResults = new List<EnduranceTestResult>();
        private TaskCompletionSource<bool> roundCompletedTcs;

        public string Url { get; set; }
        public int MinRequests { get; set; }
        public int MaxRequests { get; set; }
        public int TimeoutInSeconds { get; set; }
        public long DurationInSeconds { get; set; }
        public string TestMode { get; set; }

        public int CurrentRound { get; private set; }
        public int TotalRequestsProcessed { get; private set; }
        public int TotalSuccessfulRequests { get; private set; }
        public int TotalFailedRequests { get; private set; }
        public int TotalErrors { get; private set; }
        public double TotalCpuUsage { get; private set; }
        public double TotalRamUsage { get; private set; }
        public double TotalLoadTime { get; private set; }
        public double TotalWaitTime { get; private set; }
        public double TotalResponseTime { get; private set; }
        public int TotalResponses { get; private set; }
        public double TotalThroughput { get; private set; }

        public event EventHandler TestStarted;
        public event EventHandler TestCompleted;
        public event EventHandler<RoundCompletedEventArgs> RoundCompleted;
        public event EventHandler<ResultReceivedEventArgs> ResultReceived;

        public TestRunner()
        {
            Url = "https://example.com";
            MinRequests = 10;
            MaxRequests = 100;
            TimeoutInSeconds = 30;
            DurationInSeconds = 60;
            TestMode = "Stable";
        }

        public List<EnduranceTestResult> GetResults()
        {
            return _testResults;
        }

        public void Reset()
        {
            CurrentRound = 0;
            TotalRequestsProcessed = 0;
            TotalSuccessfulRequests = 0;
            TotalFailedRequests = 0;
            TotalErrors = 0;
            TotalCpuUsage = 0;
            TotalRamUsage = 0;
            TotalLoadTime = 0;
            TotalWaitTime = 0;
            TotalResponseTime = 0;
            TotalResponses = 0;
            TotalThroughput = 0;
            _testResults.Clear();
        }

        public async Task RunTest(CancellationToken cancellationToken)
        {
            Reset();
            HttpClient httpClient = HttpClientProvider.Instance;

            TestStarted?.Invoke(this, EventArgs.Empty);

            Stopwatch stopwatchTotal = new Stopwatch();
            stopwatchTotal.Start();

            int currentRequests = MinRequests;
            Random random = new Random();

            while (!cancellationToken.IsCancellationRequested)
            {
                CurrentRound++;
                int roundSuccessfulRequests = 0;
                int roundFailedRequests = 0;
                double roundLoadTimeSum = 0;
                double roundWaitTimeSum = 0;
                double roundResponseTime = 0;

                Stopwatch stopwatchRound = new Stopwatch();
                stopwatchRound.Start();

                switch (TestMode)
                {
                    case "Progressive":
                        double progress = stopwatchTotal.Elapsed.TotalSeconds / DurationInSeconds;
                        if (progress >= 1.0)
                        {
                            currentRequests = MaxRequests;
                        }
                        else
                        {
                            currentRequests = (int)(MinRequests + (MaxRequests - MinRequests) * progress);
                        }
                        break;

                    case "Fluctuative":
                        currentRequests = random.Next(MinRequests, MaxRequests + 1);
                        break;
                }

                var tasks = new List<Task<EnduranceTestResult>>();
                for (int i = 0; i < currentRequests; i++)
                {
                    tasks.Add(SendHttpRequest(httpClient, Url, CurrentRound, cancellationToken, currentRequests));
                }

                var results = await Task.WhenAll(tasks);
                _testResults.AddRange(results);

                double roundCpuUsage = await SystemMonitor.GetCpuUsage();
                double roundRamUsage = await SystemMonitor.GetRamUsage();

                int maxRetries = 5;
                int retryCount = 0;

                while (roundCpuUsage == 0.0 && retryCount < maxRetries)
                {
                    await Task.Delay(100);
                    roundCpuUsage = await SystemMonitor.GetCpuUsage();
                    retryCount++;
                }

                foreach (var result in results)
                {
                    ResultReceived?.Invoke(this, new ResultReceivedEventArgs(result, CurrentRound));

                    roundLoadTimeSum += (double)result.LoadTime.TotalMilliseconds;
                    roundWaitTimeSum += (double)result.WaitTime.TotalMilliseconds;
                    roundResponseTime += (double)result.ResponseTime.TotalMilliseconds;

                    if (result.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        roundSuccessfulRequests++;
                        TotalSuccessfulRequests++;
                    }
                    else
                    {
                        roundFailedRequests++;
                        TotalFailedRequests++;
                        TotalErrors++;
                    }
                }

                stopwatchRound.Stop();

                double roundDurationInSeconds = stopwatchRound.Elapsed.TotalSeconds;
                double roundDuration = Math.Min(roundDurationInSeconds, TimeoutInSeconds);
                double throughputDuration = Math.Min(roundDurationInSeconds, TimeoutInSeconds);

                if (currentRequests > 0)
                {
                    double validLoadTimeSum = 0;
                    double validWaitTimeSum = 0;
                    double validResponseTimeSum = 0;
                    int validResponseCount = 0;

                    foreach (var result in results)
                    {
                        if (result.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            validLoadTimeSum += result.LoadTime.TotalMilliseconds;
                            validWaitTimeSum += result.WaitTime.TotalMilliseconds;
                            validResponseTimeSum += result.ResponseTime.TotalMilliseconds;
                            validResponseCount++;
                        }
                        else
                        {
                            validLoadTimeSum += result.LoadTime.TotalMilliseconds;
                            validWaitTimeSum += result.WaitTime.TotalMilliseconds;
                            validResponseTimeSum += result.ResponseTime.TotalMilliseconds;
                        }
                    }

                    double averageRoundLoadTime = validResponseCount > 0 ? validLoadTimeSum / currentRequests : 0;
                    double averageRoundWaitTime = validResponseCount > 0 ? validWaitTimeSum / currentRequests : 0;
                    double averageRoundResponseTime = validResponseCount > 0 ? validResponseTimeSum / currentRequests : 0;
                    double throughput = (double)roundSuccessfulRequests / (throughputDuration > 0 ? throughputDuration : 1);
                    TotalThroughput += throughput;
                    double errorRate = (double)roundFailedRequests / currentRequests * 100;

                    foreach (var result in results)
                    {
                        result.CpuUsage = roundCpuUsage;
                        result.RamUsage = roundRamUsage;
                        result.SuccessfulRequests = roundSuccessfulRequests;
                        result.FailedRequests = roundFailedRequests;
                        result.AverageLoadTime = averageRoundLoadTime;
                        result.AverageWaitTime = averageRoundWaitTime;
                        result.AverageResponseTime = averageRoundResponseTime;
                        result.Throughput = throughput;
                        result.ErrorRate = errorRate;
                        result.RoundDuration = roundDuration;
                    }

                    TotalCpuUsage += roundCpuUsage;
                    TotalRamUsage += roundRamUsage;
                    TotalLoadTime += validLoadTimeSum;
                    TotalWaitTime += validWaitTimeSum;
                    TotalResponseTime += validResponseTimeSum;
                    TotalResponses += currentRequests;
                    TotalRequestsProcessed += currentRequests;

                    roundCompletedTcs = new TaskCompletionSource<bool>();
                    RoundCompleted?.Invoke(this, new RoundCompletedEventArgs(
                        roundCpuUsage, roundRamUsage,
                        roundSuccessfulRequests, roundFailedRequests,
                        averageRoundLoadTime, averageRoundWaitTime, averageRoundResponseTime,
                        throughput, errorRate, roundDuration, currentRequests
                    ));
                    await roundCompletedTcs.Task;
                }

                if (stopwatchTotal.Elapsed.TotalSeconds >= DurationInSeconds)
                {
                    break;
                }
            }

            stopwatchTotal.Stop();
            TestCompleted?.Invoke(this, EventArgs.Empty);
        }

        public void CompleteRound()
        {
            roundCompletedTcs?.TrySetResult(true);
        }

        private async Task<EnduranceTestResult> SendHttpRequest(HttpClient httpClient, string url, int round, CancellationToken cancellationToken, int currentRequests)
        {
            Stopwatch totalStopwatch = new Stopwatch();
            Stopwatch waitTimeStopwatch = new Stopwatch();
            Stopwatch loadTimeStopwatch = new Stopwatch();

            TimeSpan waitTime = TimeSpan.Zero;
            TimeSpan loadTime = TimeSpan.Zero;
            TimeSpan responseTime = TimeSpan.Zero;

            totalStopwatch.Start();

            try
            {
                using (var requestTimeout = new CancellationTokenSource(TimeSpan.FromSeconds(TimeoutInSeconds)))
                {
                    using (var combinedToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, requestTimeout.Token))
                    {
                        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);

                        waitTimeStopwatch.Start();

                        using (HttpResponseMessage response = await httpClient.SendCompressedAsync(request))
                        {
                            waitTime = waitTimeStopwatch.Elapsed;
                            waitTimeStopwatch.Stop();

                            loadTimeStopwatch.Start();

                            await response.Content.ReadAsStringAsync();

                            loadTime = loadTimeStopwatch.Elapsed;
                            loadTimeStopwatch.Stop();

                            responseTime = totalStopwatch.Elapsed;

                            return new EnduranceTestResult
                            {
                                StatusCode = response.StatusCode,
                                ReasonPhrase = response.ReasonPhrase,
                                LoadTime = loadTime,
                                WaitTime = waitTime,
                                ResponseTime = responseTime,
                                Round = round,
                                CpuUsage = 0,
                                RamUsage = 0,
                                SuccessfulRequests = response.StatusCode == System.Net.HttpStatusCode.OK ? 1 : 0,
                                FailedRequests = response.StatusCode != System.Net.HttpStatusCode.OK ? 1 : 0,
                                AverageLoadTime = (double)loadTime.TotalMilliseconds,
                                AverageWaitTime = (double)waitTime.TotalMilliseconds,
                                AverageResponseTime = (double)responseTime.TotalMilliseconds,
                                RequestPerRound = currentRequests
                            };
                        }
                    }
                }
            }
            catch (TaskCanceledException)
            {
                waitTimeStopwatch.Stop();
                loadTimeStopwatch.Stop();
                totalStopwatch.Stop();

                return new EnduranceTestResult
                {
                    StatusCode = System.Net.HttpStatusCode.RequestTimeout,
                    ReasonPhrase = "Request Timeout - The request was canceled due to timeout",
                    LoadTime = TimeSpan.Zero,
                    WaitTime = TimeSpan.Zero,
                    ResponseTime = TimeSpan.Zero,
                    Round = round,
                    CpuUsage = 0,
                    RamUsage = 0,
                    SuccessfulRequests = 0,
                    FailedRequests = 1,
                    AverageLoadTime = (double)loadTimeStopwatch.Elapsed.TotalMilliseconds,
                    AverageWaitTime = (double)waitTimeStopwatch.Elapsed.TotalMilliseconds,
                    AverageResponseTime = (double)totalStopwatch.Elapsed.TotalMilliseconds,
                    RequestPerRound = currentRequests
                };
            }
            catch (Exception ex)
            {
                waitTimeStopwatch.Stop();
                loadTimeStopwatch.Stop();
                totalStopwatch.Stop();

                return new EnduranceTestResult
                {
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    ReasonPhrase = ex.Message,
                    LoadTime = TimeSpan.Zero,
                    WaitTime = TimeSpan.Zero,
                    ResponseTime = TimeSpan.Zero,
                    Round = round,
                    CpuUsage = 0,
                    RamUsage = 0,
                    SuccessfulRequests = 0,
                    FailedRequests = 1,
                    AverageLoadTime = (double)loadTimeStopwatch.Elapsed.TotalMilliseconds,
                    AverageWaitTime = (double)waitTimeStopwatch.Elapsed.TotalMilliseconds,
                    AverageResponseTime = (double)totalStopwatch.Elapsed.TotalMilliseconds,
                    RequestPerRound = currentRequests
                };
            }
        }
    }
}