using System;
using System.Collections.Generic;
using ClosedXML.Excel;
using System.Diagnostics;
using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text;
using System.Linq;
using System.Drawing;
using System.IO;

namespace Endurance_Testing
{
    public partial class EnduranceTesting : Form
    {
        private CancellationTokenSource cancellationTokenSource;
        private List<EnduranceTestResult> enduranceTestResults = new List<EnduranceTestResult>();
        private int totalRequestsProcessed;
        private int totalRequests;
        private int maxRequests;
        private int timeoutInSeconds;
        private long durationInSeconds;
        private int currentRound;
        private int totalSuccessfulRequests;
        private int totalFailedRequests;
        private int totalErrors;
        private double totalCpuUsage;
        private double totalRamUsage;
        private double totalLoadTime;
        private double totalWaitTime;
        private double totalResponseTime;
        private int totalResponses;
        private double totalThroughput;
        private bool isRunning = false;
        private string selectedTimePeriod;

        public EnduranceTesting()
        {
            InitializeComponent();
            this.Load += EnduranceTesting_Load;
            this.FormClosing += EnduranceTesting_FormClosing;
            textBoxInputRequest.KeyPress += new KeyPressEventHandler(textBoxOnlyNumber_KeyPress);
            textBoxInputMaxRequest.KeyPress += new KeyPressEventHandler(textBoxOnlyNumber_KeyPress);
            textBoxTimeout.KeyPress += new KeyPressEventHandler(textBoxOnlyNumber_KeyPress);
            textBoxTime.KeyPress += new KeyPressEventHandler(textBoxOnlyNumber_KeyPress);
            comboBoxMode.SelectedIndex = 0;
            comboBoxMode.SelectedIndexChanged += new EventHandler(comboBoxMode_SelectedIndexChanged);
        }

        private void EnduranceTesting_FormClosing(object sender, FormClosingEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to exit the application?",
                                           "Confirm Exit",
                                           MessageBoxButtons.YesNo,
                                           MessageBoxIcon.Warning);

            if (result == DialogResult.No)
            {
                e.Cancel = true;
            }
        }

        private void EnduranceTesting_Load(object sender, EventArgs e)
        {
            textBoxInputUrl.Text = "https://example.com";
            btnStop.Enabled = false;
            btnExport.Enabled = false;
            selectedTimePeriod = "second(s)";
        }

        private void textBoxOnlyNumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void textBoxInputUrl_TextChanged(object sender, EventArgs e)
        {
            if (textBoxInputUrl.Text.Length > 0 && !isRunning)
            {
                btnClear.Enabled = true;
            }
        }

        private void textBoxInputRequest_TextChanged(object sender, EventArgs e)
        {
            if (textBoxInputRequest.Text.Length > 0 && !isRunning)
            {
                btnClear.Enabled = true;
            }

            if (textBoxInputRequest.Text.Length > 8)
            {
                MessageBox.Show("Input request should be limited to 8 digits.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBoxInputRequest.Text = textBoxInputRequest.Text.Substring(0, 8);
                textBoxInputRequest.SelectionStart = textBoxInputRequest.Text.Length;
            }
        }

        private void textBoxInputMaxRequest_TextChanged(object sender, EventArgs e)
        {
            if (textBoxInputMaxRequest.Text.Length > 0 && !isRunning)
            {
                btnClear.Enabled = true;
            }

            if (textBoxInputMaxRequest.Text.Length > 8)
            {
                MessageBox.Show("Input request should be limited to 8 digits.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBoxInputMaxRequest.Text = textBoxInputMaxRequest.Text.Substring(0, 8);
                textBoxInputMaxRequest.SelectionStart = textBoxInputMaxRequest.Text.Length;
            }
        }

        private void comboBoxMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedMode = comboBoxMode.SelectedItem.ToString();

            if (selectedMode == "Stable")
            {
                textBoxInputMaxRequest.Enabled = false;
                textBoxInputMaxRequest.Clear();
            }
            else
            {
                textBoxInputMaxRequest.Enabled = true;
            }
        }

        private void textBoxTimeout_TextChanged(object sender, EventArgs e)
        {
            if (textBoxTimeout.Text.Length > 0 && !isRunning)
            {
                btnClear.Enabled = true;
            }

            if (textBoxTimeout.Text.Length > 8)
            {
                MessageBox.Show("Input timeout should be limited to 8 digits.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBoxTimeout.Text = textBoxTimeout.Text.Substring(0, 8);
                textBoxTimeout.SelectionStart = textBoxTimeout.Text.Length;
            }

        }

        private void textBoxTime_TextChanged(object sender, EventArgs e)
        {
            if (textBoxTime.Text.Length > 0 && !isRunning)
            {
                btnClear.Enabled = true;
            }

            if (textBoxTime.Text.Length > 8)
            {
                MessageBox.Show("Input time in period should be limited to 8 digits.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBoxTime.Text = textBoxTime.Text.Substring(0, 8);
                textBoxTime.SelectionStart = textBoxTime.Text.Length;
            }
        }

        private bool IsValidUrl(string url)
        {
            Uri uriResult;
            bool result = Uri.TryCreate(url, UriKind.Absolute, out uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            return result;
        }

        private async void btnStart_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(textBoxOutput.Text))
            {
                var result = MessageBox.Show("There are results in the output. If you continue, the current results will be cleared. Do you want to continue?",
                                               "Warning",
                                               MessageBoxButtons.YesNo,
                                               MessageBoxIcon.Warning);

                if (result == DialogResult.No)
                {
                    return;
                }
            }

            isRunning = true;

            string url = textBoxInputUrl.Text;
            if (string.IsNullOrWhiteSpace(url) || !IsValidUrl(url))
            {
                MessageBox.Show("Please enter a valid URL.");
                isRunning = false;
                return;
            }

            if (!int.TryParse(textBoxInputRequest.Text, out totalRequests) || totalRequests <= 0)
            {
                MessageBox.Show("Please enter a valid number of requests.");
                isRunning = false;
                return;
            }

            string selectedMode = comboBoxMode.SelectedItem.ToString();

            if (selectedMode != "Stable")
            {
                if (!int.TryParse(textBoxInputMaxRequest.Text, out maxRequests) || maxRequests <= 0)
                {
                    MessageBox.Show("Please enter a valid maximum number of requests.");
                    isRunning = false;
                    return;
                }

                if (maxRequests <= totalRequests)
                {
                    MessageBox.Show("Maximum requests must be greater than minimum requests.");
                    isRunning = false;
                    return;
                }
            }

            if (!int.TryParse(textBoxTimeout.Text, out timeoutInSeconds) || timeoutInSeconds <= 0)
            {
                MessageBox.Show("Please enter a valid timeout.");
                isRunning = false;
                return;
            }

            if (!long.TryParse(textBoxTime.Text, out durationInSeconds) || durationInSeconds <= 0)
            {
                MessageBox.Show("Please enter a valid duration.");
                isRunning = false;
                return;
            }

            if (radioButtonMinute.Checked)
            {
                durationInSeconds *= 60;
                selectedTimePeriod = "minute(s)";
            }
            else if (radioButtonHour.Checked)
            {
                durationInSeconds *= 3600;
                selectedTimePeriod = "hour(s)";
            }
            else if (radioButtonSecond.Checked)
            {
                selectedTimePeriod = "second(s)";
            }
            else if (!radioButtonSecond.Checked)
            {
                MessageBox.Show("Please select a time period (seconds, minutes, or hours).");
                isRunning = false;
                return;
            }

            long maxDurationInSeconds = 86400 * 365;
            if (durationInSeconds > maxDurationInSeconds)
            {
                MessageBox.Show("Duration exceeds the maximum limit of 1 year (which is 8760 hours, 525600 minutes, or 31536000 seconds).");
                isRunning = false;
                return;
            }

            textBoxOutput.Clear();
            lblTimeLeft.Text = "00:00:00:00";

            enduranceTestResults.Clear();

            btnStart.Enabled = false;
            btnStop.Enabled = true;
            btnClear.Enabled = false;
            btnClear.Enabled = false;
            btnExport.Enabled = false;

            cancellationTokenSource = new CancellationTokenSource();
            currentRound = 0;
            totalSuccessfulRequests = 0;
            totalFailedRequests = 0;
            totalCpuUsage = 0;
            totalRamUsage = 0;
            totalLoadTime = 0;
            totalWaitTime = 0;
            totalResponseTime = 0;
            totalResponses = 0;
            totalRequestsProcessed = 0;
            totalErrors = 0;
            totalThroughput = 0;

            var countdownTask = StartCountdown(durationInSeconds, cancellationTokenSource.Token);

            try
            {
                await RunEnduranceTest(url, cancellationTokenSource.Token);

                try
                {
                    await countdownTask;
                }
                catch (TaskCanceledException)
                {
                    
                }
            }
            catch (TaskCanceledException)
            {
                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occured: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                isRunning = false;
                btnClear.Enabled = true;
                btnExport.Enabled = true;

                if (currentRound > 0)
                {
                    ShowSummary();
                }
            }
        }

        private async Task StartCountdown(long durationInSeconds, CancellationToken cancellationToken)
        {
            DateTime endTime = DateTime.Now.AddSeconds(durationInSeconds);

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    TimeSpan remainingTime = endTime - DateTime.Now;

                    if (remainingTime.TotalSeconds <= 0)
                    {
                        lblTimeLeft.Text = "00:00:00:00";

                        if (!cancellationToken.IsCancellationRequested)
                        {
                            cancellationTokenSource.Cancel();
                        }
                        break;
                    }

                    lblTimeLeft.Text = $"{(int)remainingTime.TotalDays}:{remainingTime.Hours:D2}:{remainingTime.Minutes:D2}:{remainingTime.Seconds:D2}";

                    await Task.Delay(100, cancellationToken);
                }
            }
            catch (TaskCanceledException)
            {
                
            }
            finally
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    lblTimeLeft.Text = "00:00:00:00";
                }
            }
        }

        private async Task RunEnduranceTest(string url, CancellationToken cancellationToken)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                Stopwatch stopwatchTotal = new Stopwatch();
                stopwatchTotal.Start();

                string selectedMode = comboBoxMode.SelectedItem.ToString();
                Random random = new Random();

                int currentRequests = totalRequests;

                while (!cancellationToken.IsCancellationRequested)
                {
                    currentRound++;
                    int roundSuccessfulRequests = 0;
                    int roundFailedRequests = 0;
                    double roundLoadTimeSum = 0;
                    double roundWaitTimeSum = 0;
                    double roundResponseTime = 0;

                    Stopwatch stopwatchRound = new Stopwatch();
                    stopwatchRound.Start();

                    switch (selectedMode)
                    {
                        case "Progressive":
                            double progress = stopwatchTotal.Elapsed.TotalSeconds / durationInSeconds;
                            if (progress >= 1.0)
                            {
                                currentRequests = maxRequests;
                            }
                            else
                            {
                                currentRequests = (int)(totalRequests + (maxRequests - totalRequests) * progress);
                            }
                            break;

                        case "Fluctuative":
                            currentRequests = random.Next(totalRequests, maxRequests + 1);
                            break;
                    }

                    var tasks = new List<Task<EnduranceTestResult>>();
                    for (int i = 0; i < currentRequests; i++)
                    {
                        tasks.Add(SendHttpRequest(httpClient, url, currentRound, cancellationToken, currentRequests));
                    }

                    var results = await Task.WhenAll(tasks);
                    enduranceTestResults.AddRange(results);

                    double roundCpuUsage = GetCpuUsage();
                    double roundRamUsage = GetRamUsage();

                    foreach (var result in results)
                    {
                        DisplayResult(result, currentRound);
                        roundLoadTimeSum += (double)result.WaitTime.TotalMilliseconds;
                        roundWaitTimeSum += (double)result.WaitTime.TotalMilliseconds;
                        roundResponseTime += (double)result.ResponseTime.TotalMilliseconds;

                        if (result.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            roundSuccessfulRequests++;
                            totalSuccessfulRequests++;
                        }
                        else
                        {
                            roundFailedRequests++;
                            totalFailedRequests++;
                            totalErrors++;
                        }
                    }
                    stopwatchRound.Stop();

                    double roundDurationInSeconds = stopwatchRound.Elapsed.TotalSeconds;
                    double roundDuration = Math.Min(roundDurationInSeconds, timeoutInSeconds);
                    double throughputDuration = Math.Min(roundDurationInSeconds, timeoutInSeconds);

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
                        double throughput = (double)roundSuccessfulRequests / (throughputDuration > 0 ? (double)throughputDuration : 1);
                        totalThroughput += throughput;
                        double errorRate = (double)roundFailedRequests / (double)currentRequests * 100;

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

                        totalCpuUsage += roundCpuUsage;
                        totalRamUsage += roundRamUsage;
                        totalLoadTime += validLoadTimeSum;
                        totalWaitTime += validWaitTimeSum;
                        totalResponseTime += validResponseTimeSum;
                        totalResponses += currentRequests;
                        totalRequestsProcessed += currentRequests;

                        DisplayRoundStatistics(roundCpuUsage,
                                               roundRamUsage,
                                               roundSuccessfulRequests,
                                               roundFailedRequests,
                                               averageRoundLoadTime,
                                               averageRoundWaitTime,
                                               averageRoundResponseTime,
                                               throughput,
                                               errorRate,
                                               roundDuration,
                                               currentRequests);
                    }


                    if (stopwatchTotal.Elapsed.TotalSeconds >= durationInSeconds)
                    {
                        cancellationTokenSource.Cancel();
                        break;
                    }
                }

                stopwatchTotal.Stop();
                btnStart.Enabled = true;
                btnStop.Enabled = false;
            }
        }

        private async Task<EnduranceTestResult> SendHttpRequest(HttpClient httpClient, string url, int round, CancellationToken cancellationToken,int currentRequests)
        {
            Stopwatch loadTimeStopwatch = new Stopwatch();
            Stopwatch waitTimeStopwatch = new Stopwatch();
            Stopwatch totalStopwatch = new Stopwatch();

            loadTimeStopwatch.Start();
            totalStopwatch.Start();

            TimeSpan loadTime = TimeSpan.Zero;
            TimeSpan waitTime = TimeSpan.Zero;

            try
            {
                using (var requestTimeout = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutInSeconds)))
                {
                    using (var combinedToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, requestTimeout.Token))
                    {
                        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);

                        using (HttpResponseMessage response = await httpClient.SendAsync(request,
                                HttpCompletionOption.ResponseHeadersRead, combinedToken.Token))
                        {
                            loadTime = loadTimeStopwatch.Elapsed;
                            loadTimeStopwatch.Stop();
                            
                            waitTimeStopwatch.Start();

                            await response.Content.ReadAsStringAsync();

                            waitTime = waitTimeStopwatch.Elapsed;

                            TimeSpan responseTime = totalStopwatch.Elapsed;

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
                                AverageResponseTime = (double)responseTime.TotalMilliseconds,
                                RequestPerRound = currentRequests
                            };
                        }
                    }
                }
            }
            catch (TaskCanceledException ex)
            {
                return new EnduranceTestResult
                {
                    StatusCode = System.Net.HttpStatusCode.RequestTimeout,
                    ReasonPhrase = "Request Timeout - The request was canceled due to timeout",
                    LoadTime = loadTimeStopwatch.Elapsed,
                    WaitTime = waitTimeStopwatch.Elapsed,
                    ResponseTime = totalStopwatch.Elapsed,
                    Round = round,
                    CpuUsage = 0,
                    RamUsage = 0,
                    SuccessfulRequests = 0,
                    FailedRequests = 1,
                    AverageResponseTime = 0,
                    RequestPerRound = currentRequests
                };
            }
            catch (Exception ex)
            {
                return new EnduranceTestResult
                {
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    ReasonPhrase = ex.Message,
                    LoadTime = loadTimeStopwatch.Elapsed,
                    WaitTime = waitTimeStopwatch.Elapsed,
                    ResponseTime = totalStopwatch.Elapsed,
                    Round = round,
                    CpuUsage = 0,
                    RamUsage = 0,
                    SuccessfulRequests = 0,
                    FailedRequests = 1,
                    AverageResponseTime = (double)totalStopwatch.Elapsed.TotalMilliseconds,
                    RequestPerRound = currentRequests
                };
            }
            finally
            {
                loadTimeStopwatch.Stop();
                waitTimeStopwatch.Stop();
                totalStopwatch.Stop();
            }
        }

        private void DisplayResult(EnduranceTestResult result, int round)
        {
            string resultString = $"Round {round}: Status: {(int)result.StatusCode}, Reason: {result.ReasonPhrase}, Load Time: {result.LoadTime.TotalMilliseconds} ms, Wait Time: {result.WaitTime.TotalMilliseconds} ms, Response Time: {result.ResponseTime.TotalMilliseconds} ms";
            textBoxOutput.AppendText(resultString + Environment.NewLine);
            textBoxOutput.ScrollToCaret();
        }

        private void DisplayRoundStatistics(double currentCpuUsage,
                                            double currentRamUsage,
                                            int roundSuccessfulRequests,
                                            int roundFailedRequests,
                                            double averageRoundLoadTime,
                                            double averageRoundWaitTime,
                                            double averageRoundResponseTime,
                                            double throughput,
                                            double errorRate,
                                            double roundDuration,
                                            int currentRequests)
        {
            string roundStats = $"Round {currentRound} Statistics:{Environment.NewLine}" +
                                $"Computer's CPU Usage: {currentCpuUsage}%{Environment.NewLine}" +
                                $"Computer's RAM Usage: {currentRamUsage} MB{Environment.NewLine}" +
                                $"Total Requests: {currentRequests}{Environment.NewLine}" +
                                $"Successful Requests: {roundSuccessfulRequests}{Environment.NewLine}" +
                                $"Failed Requests: {roundFailedRequests}{Environment.NewLine}" +
                                $"Average Load Time: {averageRoundLoadTime} ms{Environment.NewLine}" +
                                $"Average Wait Time: {averageRoundWaitTime} ms{Environment.NewLine}" +
                                $"Average Response Time: {averageRoundResponseTime} ms{Environment.NewLine}" +
                                $"Throughput: {throughput} requests/second{Environment.NewLine}" +
                                $"Error Rate: {errorRate}%{Environment.NewLine}" +
                                $"Round Duration: {roundDuration} seconds{Environment.NewLine}{Environment.NewLine}";

            textBoxOutput.AppendText(roundStats);
            textBoxOutput.ScrollToCaret();
        }

        private void ShowSummary()
        {
            double averageCpuUsage = currentRound > 0 ? totalCpuUsage / currentRound : 0;
            double averageRamUsage = currentRound > 0 ? totalRamUsage / currentRound : 0;

            double averageLoadTime = totalResponses > 0 ? totalLoadTime / totalResponses : 0;
            double averageWaitTime = totalResponses > 0 ? totalWaitTime / totalResponses : 0;
            double averageResponseTime = totalResponses > 0 ? totalResponseTime / totalResponses : 0;
            double averageThroughput = totalResponses > 0 ? totalThroughput / currentRound : 0;
            double averageRoundDuration = currentRound > 0 ? enduranceTestResults.Average(result => result.RoundDuration) : 0;

            string summaryMessage = $"Summary:{Environment.NewLine}" +
                                    $"Total Requests: {totalRequestsProcessed}{Environment.NewLine}" +
                                    $"Successful Requests: {totalSuccessfulRequests}{Environment.NewLine}" +
                                    $"Failed Requests: {totalFailedRequests}{Environment.NewLine}" +
                                    $"Average Computer's CPU Usage: {averageCpuUsage}%{Environment.NewLine}" +
                                    $"Average Computer's RAM Usage: {averageRamUsage} MB{Environment.NewLine}" +
                                    $"Average Load Time: {averageLoadTime} ms{Environment.NewLine}" +
                                    $"Average Wait Time: {averageWaitTime} ms{Environment.NewLine}" +
                                    $"Average Response Time: {averageResponseTime} ms{Environment.NewLine}" +
                                    $"Average Throughput: {averageThroughput} requests/second{Environment.NewLine}" +
                                    $"Average Error Rate: {(totalFailedRequests / (double)totalRequestsProcessed) * 100}%{Environment.NewLine}" +
                                    $"Average Round Duration: {averageRoundDuration} seconds";

            textBoxOutput.AppendText(summaryMessage);
            textBoxOutput.ScrollToCaret();
        }

        private double GetCpuUsage()
        {
            using (var cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total"))
            {
                cpuCounter.NextValue();
                Thread.Sleep(1000);
                return cpuCounter.NextValue();
            }
        }

        private double GetRamUsage()
        {
            using (Process currentProcess = Process.GetCurrentProcess())
            {
                return currentProcess.WorkingSet64 / (1024 * 1024.0);
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to stop the endurance test?",
                                           "Confirm Stop",
                                           MessageBoxButtons.YesNo,
                                           MessageBoxIcon.Warning);

            if (result == DialogResult.No)
            {
                return;
            }

            if (cancellationTokenSource != null && !cancellationTokenSource.IsCancellationRequested)
            {
                cancellationTokenSource.Cancel();
            }

            cancellationTokenSource.Cancel();
            isRunning = false;
            btnStart.Enabled = true;
            btnStop.Enabled = false;
            btnClear.Enabled = true;
            btnExport.Enabled = true;
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            ShowHelpMessageBox();
        }

        private void ShowHelpMessageBox()
        {
            string helpMessage = GenerateHelpMessage();
            MessageBox.Show(helpMessage, "User Guide", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private string GenerateHelpMessage()
        {
            StringBuilder helpMessage = new StringBuilder();

            helpMessage.AppendLine("User Guide for Endurance Testing Application:");
            helpMessage.AppendLine();
            helpMessage.AppendLine("1. Enter the target URL you wish to test in the text field labeled 'URL:'.");
            helpMessage.AppendLine("2. Enter the minimum number of requests to dispatch per round in the text field labeled 'Number of Request (Min and Max)' (maximum 8 digits).");
            helpMessage.AppendLine("   - For 'Progressive' or 'Fluctuative' modes, also enter the maximum number of requests to dispatch per round in the same field.");
            helpMessage.AppendLine("3. Enter the timeout threshold in seconds for each test round in the text field labeled 'Timeout Per-Round (In Seconds):'.");
            helpMessage.AppendLine("4. Select the desired test mode from the dropdown menu labeled 'Mode:' (Stable, Progressive, or Fluctuative).");
            helpMessage.AppendLine("5. Enter the test duration in the 'Time in Period:' field and select the unit of time (seconds, minutes, or hours) using the radio buttons to the right of this field.");
            helpMessage.AppendLine("   - Stable: Dispatches a consistent number of requests in each test round.");
            helpMessage.AppendLine("   - Progressive: Gradually increases the number of requests per round over the duration of the test.");
            helpMessage.AppendLine("   - Fluctuative: Dispatches a random number of requests within the defined minimum and maximum range for each round.");
            helpMessage.AppendLine("6. Click the 'Start' button to initiate the endurance test.");
            helpMessage.AppendLine("7. Monitor the test results in the 'Output:' text area below the input fields and the remaining time above the output area.");
            helpMessage.AppendLine("8. Upon test completion, the 'Output:' area will display:");
            helpMessage.AppendLine("    - Total Requests: The total number of requests sent during the test.");
            helpMessage.AppendLine("    - Successful Requests: The number of requests that received a successful HTTP 200 (OK) response.");
            helpMessage.AppendLine("    - Failed Requests: The number of requests that did not receive an HTTP 200 (OK) response or timed out.");
            helpMessage.AppendLine("    - Average Computer's CPU Usage: The average percentage of computer's CPU utilization during the test.");
            helpMessage.AppendLine("    - Average Computer's RAM Usage: The average computer's RAM utilization in megabytes during the test.");
            helpMessage.AppendLine("    - Average Response Time: The average response time for all requests (including successful and failed).");
            helpMessage.AppendLine("    - Average Throughput: The average number of requests processed per second.");
            helpMessage.AppendLine("    - Average Error Rate: The percentage of requests that failed or timed out.");
            helpMessage.AppendLine("    - Average Round Duration: The average time in seconds it takes to complete one round of requests.");
            helpMessage.AppendLine("9. Click the 'Clear' button to reset the input fields and the output area.");
            helpMessage.AppendLine("10. Optionally, click the 'Export' button to export the test results to an Excel file.");
            helpMessage.AppendLine();
            helpMessage.AppendLine("Note:");
            helpMessage.AppendLine("   - Ensure that your internet connection is stable and reliable for conducting this test.");
            helpMessage.AppendLine("   - Be aware that device performance may be reduced during the testing process and confirm that your device specifications are adequate.");
            helpMessage.AppendLine("   - The actual test duration may vary slightly from the input time due to the processing time for handling requests and responses.");

            return helpMessage.ToString();
        }

        private void btnInfo_Click(object sender, EventArgs e)
        {
            ShowInfoMessageBox();
        }

        private void ShowInfoMessageBox()
        {
            string infoMessage = GenerateInfoMessage();
            MessageBox.Show(infoMessage, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private string GenerateInfoMessage()
        {
            StringBuilder infoMessage = new StringBuilder();

            infoMessage.AppendLine("Information Regarding Endurance Testing:");
            infoMessage.AppendLine();
            infoMessage.AppendLine("\"Endurance testing, also known as soak testing, involves subjecting an application to a sustained load for an extended period. This methodology helps uncover memory leaks, resource depletion, and other performance degradation issues that might only surface after prolonged usage.\"[1]");
            infoMessage.AppendLine("[1] S. Pargaonkar, \"A Comprehensive Review of Performance Testing Methodologies and Best Practices: Software Quality Engineering,\" International Journal of Science and Research (IJSR), vol. 12, no. 8, pp. 2008-2014, August 2023.");
            infoMessage.AppendLine();
            infoMessage.AppendLine("Testing Metrics:");
            infoMessage.AppendLine();
            infoMessage.AppendLine("1. Computer's CPU Usage:");
            infoMessage.AppendLine("    a. Description:");
            infoMessage.AppendLine("        The computer's CPU usage metric represents the percentage of the computer's processing power consumed during the endurance test.");
            infoMessage.AppendLine("    b. Formula:");
            infoMessage.AppendLine("        Computer's CPU Usage = Current Computer's CPU Utilization Percentage");
            infoMessage.AppendLine("2. Computer's RAM Usage:");
            infoMessage.AppendLine("    a. Description:");
            infoMessage.AppendLine("        The computer's RAM usage metric indicates the amount of computer memory utilized during the endurance test.");
            infoMessage.AppendLine("    b. Formula:");
            infoMessage.AppendLine("        Computer's RAM Usage = Current Computer's RAM Utilization in Megabytes");
            infoMessage.AppendLine("3. Total Requests:");
            infoMessage.AppendLine("    a. Description:");
            infoMessage.AppendLine("        Total Requests is the aggregate count of requests dispatched during the endurance test.");
            infoMessage.AppendLine("    b. Formula:");
            infoMessage.AppendLine("        Total Requests = Sum of Requests Dispatched");
            infoMessage.AppendLine("4. Successful Requests:");
            infoMessage.AppendLine("    a. Description:");
            infoMessage.AppendLine("        Successful Requests is the count of requests that receive an HTTP 200 (OK) response.");
            infoMessage.AppendLine("    b. Formula:");
            infoMessage.AppendLine("        Successful Requests = Count of Requests with HTTP Status Code 200 (OK)");
            infoMessage.AppendLine("5. Failed Requests:");
            infoMessage.AppendLine("    a. Description:");
            infoMessage.AppendLine("        Failed Requests is the count of requests that fail, calculated as the difference between Total Requests and Successful Requests.");
            infoMessage.AppendLine("    b. Formula:");
            infoMessage.AppendLine("        Failed Requests = Total Requests − Successful Requests");
            infoMessage.AppendLine("6. Average Response Time:");
            infoMessage.AppendLine("    a. Description:");
            infoMessage.AppendLine("        Average Response Time represents the mean response time per request during the endurance test (including successful and failed requests).");
            infoMessage.AppendLine("    b. Formula:");
            infoMessage.AppendLine("        Average Response Time = Total Response Time / Total Requests");
            infoMessage.AppendLine("7. Throughput:");
            infoMessage.AppendLine("    a. Description:");
            infoMessage.AppendLine("        Throughput measures the number of successful requests processed per second during the endurance test.");
            infoMessage.AppendLine("    b. Formula:");
            infoMessage.AppendLine("        Throughput = Successful Requests / Total Test Duration (in seconds)");
            infoMessage.AppendLine("8. Error Rate:");
            infoMessage.AppendLine("    a. Description:");
            infoMessage.AppendLine("        Error Rate indicates the percentage of requests that failed during the endurance test.");
            infoMessage.AppendLine("    b. Formula:");
            infoMessage.AppendLine("        Error Rate = (Failed Requests / Total Requests) * 100");
            infoMessage.AppendLine("9. Round Duration:");
            infoMessage.AppendLine("   a. Description:");
            infoMessage.AppendLine("        Round duration indicates the actual duration for each testing round, capped by the timeout value.");
            infoMessage.AppendLine("   b. Formula:");
            infoMessage.AppendLine("        Round Duration = Round Time, but if it exceeds Timeout Duration then Round Time = Timeout Duration");

            return infoMessage.ToString();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to clear all inputs and results? This action cannot be undone.",
                                           "Confirm Clear",
                                           MessageBoxButtons.YesNo,
                                           MessageBoxIcon.Warning);

            if (result == DialogResult.No)
            {
                return;
            }

            textBoxInputUrl.Clear();
            textBoxInputRequest.Clear();
            textBoxInputMaxRequest.Clear();
            textBoxTime.Clear();
            textBoxTimeout.Clear();
            lblTimeLeft.Text = "00:00:00:00";
            textBoxOutput.Clear();
            enduranceTestResults.Clear();
            totalRequests = 0;
            totalRequestsProcessed = 0;
            durationInSeconds = 0;
            btnStart.Enabled = true;
            btnStop.Enabled = false;
            btnClear.Enabled = false;
            btnExport.Enabled = false;
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            string url = textBoxInputUrl.Text;

            if (enduranceTestResults.Count == 0)
            {
                MessageBox.Show("There is no test result to export.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ContextMenuStrip exportMenu = new ContextMenuStrip();
            exportMenu.Items.Add("Excel (.xlsx)", null, (s, args) => ExportToExcel(url));
            exportMenu.Items.Add("CSV (.csv)", null, (s, args) => ExportToCsv(url));
            exportMenu.Items.Add("JSON (.json)", null, (s, args) => ExportToJson(url));
            exportMenu.Items.Add("HTML Report (.html)", null, (s, args) => ExportToHtml(url));

            exportMenu.Show(btnExport, new Point(0, btnExport.Height));
        }

        private void ExportToExcel(string url)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Endurance Test Results");

                worksheet.Cell(1, 1).Value = $"Endurance Testing Results from URL: {url}";
                worksheet.Cell(1, 1).Style.Font.Bold = true;
                worksheet.Cell(1, 1).Style.Font.FontSize = 14;
                worksheet.Cell(1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Range("A1:Q1").Merge();

                worksheet.Cell(2, 1).Value = "Round";
                worksheet.Cell(2, 2).Value = "Status";
                worksheet.Cell(2, 3).Value = "Reason";
                worksheet.Cell(2, 4).Value = "Load Time (ms)";
                worksheet.Cell(2, 5).Value = "Wait Time (ms)";
                worksheet.Cell(2, 6).Value = "Response Time (ms)";
                worksheet.Cell(2, 7).Value = "Computer's CPU Usage (%)";
                worksheet.Cell(2, 8).Value = "Computer's RAM Usage (MB)";
                worksheet.Cell(2, 9).Value = "Total Requests";
                worksheet.Cell(2, 10).Value = "Successful Requests";
                worksheet.Cell(2, 11).Value = "Failed Requests";
                worksheet.Cell(2, 12).Value = "Average Load Time (ms)";
                worksheet.Cell(2, 13).Value = "Average Wait Time (ms)";
                worksheet.Cell(2, 14).Value = "Average Response Time (ms)";
                worksheet.Cell(2, 15).Value = "Throughput (requests/sec)";
                worksheet.Cell(2, 16).Value = "Error Rate (%)";
                worksheet.Cell(2, 17).Value = "Round Duration (seconds)";

                worksheet.SheetView.Freeze(2, 1);

                int row = 3;
                foreach (var result in enduranceTestResults)
                {
                    worksheet.Cell(row, 1).Value = result.Round;
                    worksheet.Cell(row, 2).Value = (int)result.StatusCode;
                    worksheet.Cell(row, 3).Value = result.ReasonPhrase;
                    worksheet.Cell(row, 4).Value = result.LoadTime.TotalMilliseconds;
                    worksheet.Cell(row, 5).Value = result.WaitTime.TotalMilliseconds;
                    worksheet.Cell(row, 6).Value = result.ResponseTime.TotalMilliseconds;
                    worksheet.Cell(row, 7).Value = result.CpuUsage;
                    worksheet.Cell(row, 8).Value = result.RamUsage;
                    worksheet.Cell(row, 9).Value = result.RequestPerRound;
                    worksheet.Cell(row, 10).Value = result.SuccessfulRequests;
                    worksheet.Cell(row, 11).Value = result.FailedRequests;
                    worksheet.Cell(row, 12).Value = result.AverageLoadTime;
                    worksheet.Cell(row, 13).Value = result.AverageWaitTime;
                    worksheet.Cell(row, 14).Value = result.AverageResponseTime;
                    worksheet.Cell(row, 15).Value = result.Throughput;
                    worksheet.Cell(row, 16).Value = result.ErrorRate;
                    worksheet.Cell(row, 17).Value = result.RoundDuration;
                    row++;
                }

                int summaryStartRow = 1;
                int summaryStartColumn = 19;

                worksheet.Cell(summaryStartRow, summaryStartColumn).Value = "Overall Summary";
                worksheet.Cell(summaryStartRow, summaryStartColumn).Style.Font.Bold = true;
                worksheet.Cell(summaryStartRow, summaryStartColumn).Style.Font.FontSize = 14;
                worksheet.Cell(summaryStartRow, summaryStartColumn).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Range("S1:T1").Merge();


                double averageCpuUsageOverall = currentRound > 0 ? totalCpuUsage / currentRound : 0;
                double averageRamUsageOverall = currentRound > 0 ? totalRamUsage / currentRound : 0;
                double averageLoadTimeOverall = totalResponses > 0 ? totalLoadTime / totalResponses : 0;
                double averageWaitTimeOverall = totalResponses > 0 ? totalWaitTime / totalResponses : 0;
                double averageResponseTimeOverall = totalResponses > 0 ? totalResponseTime / totalResponses : 0;
                double averageThroughputOverall = totalResponses > 0 ? totalThroughput / currentRound : 0;
                double averageErrorRateOverall = totalRequestsProcessed > 0 ? (double)totalFailedRequests / totalRequestsProcessed * 100 : 0;
                double averageRoundDurationOverall = currentRound > 0 ? enduranceTestResults.Average(result => result.RoundDuration) : 0;

                long durationTimePeriod = 0;

                if (selectedTimePeriod == "hour(s)")
                    durationTimePeriod = durationInSeconds / 3600;
                else if (selectedTimePeriod == "minute(s)")
                    durationTimePeriod = durationInSeconds / 60;
                else if (selectedTimePeriod == "second(s)")
                    durationTimePeriod = durationInSeconds;

                summaryStartRow++;
                worksheet.Cell(summaryStartRow, summaryStartColumn).Value = "Total Requests:";
                worksheet.Cell(summaryStartRow, summaryStartColumn + 1).Value = totalRequestsProcessed;

                summaryStartRow++;
                worksheet.Cell(summaryStartRow, summaryStartColumn).Value = "Successful Requests:";
                worksheet.Cell(summaryStartRow, summaryStartColumn + 1).Value = totalSuccessfulRequests;

                summaryStartRow++;
                worksheet.Cell(summaryStartRow, summaryStartColumn).Value = "Failed Requests:";
                worksheet.Cell(summaryStartRow, summaryStartColumn + 1).Value = totalFailedRequests;

                summaryStartRow++;
                worksheet.Cell(summaryStartRow, summaryStartColumn).Value = "Average Computer's CPU Usage:";
                worksheet.Cell(summaryStartRow, summaryStartColumn + 1).Value = averageCpuUsageOverall.ToString() + "%";

                summaryStartRow++;
                worksheet.Cell(summaryStartRow, summaryStartColumn).Value = "Average Computer's RAM Usage:";
                worksheet.Cell(summaryStartRow, summaryStartColumn + 1).Value = averageRamUsageOverall.ToString() + " MB";

                summaryStartRow++;
                worksheet.Cell(summaryStartRow, summaryStartColumn).Value = "Average Load Time:";
                worksheet.Cell(summaryStartRow, summaryStartColumn + 1).Value = averageLoadTimeOverall.ToString() + " ms";

                summaryStartRow++;
                worksheet.Cell(summaryStartRow, summaryStartColumn).Value = "Average Wait Time:";
                worksheet.Cell(summaryStartRow, summaryStartColumn + 1).Value = averageWaitTimeOverall.ToString() + " ms";

                summaryStartRow++;
                worksheet.Cell(summaryStartRow, summaryStartColumn).Value = "Average Response Time:";
                worksheet.Cell(summaryStartRow, summaryStartColumn + 1).Value = averageResponseTimeOverall.ToString() + " ms";

                summaryStartRow++;
                worksheet.Cell(summaryStartRow, summaryStartColumn).Value = "Average Throughput:";
                worksheet.Cell(summaryStartRow, summaryStartColumn + 1).Value = averageThroughputOverall.ToString() + " requests/sec";

                summaryStartRow++;
                worksheet.Cell(summaryStartRow, summaryStartColumn).Value = "Average Error Rate:";
                worksheet.Cell(summaryStartRow, summaryStartColumn + 1).Value = averageErrorRateOverall.ToString("F2") + "%";

                summaryStartRow++;
                worksheet.Cell(summaryStartRow, summaryStartColumn).Value = "Average Round Duration:";
                worksheet.Cell(summaryStartRow, summaryStartColumn + 1).Value = averageRoundDurationOverall.ToString() + " seconds";

                int paramStartRow = 1;
                int paramStartColumn = 22;

                worksheet.Cell(paramStartRow, paramStartColumn).Value = "Test Parameter";
                worksheet.Cell(paramStartRow, paramStartColumn).Style.Font.Bold = true;
                worksheet.Cell(paramStartRow, paramStartColumn).Style.Font.FontSize = 14;
                worksheet.Cell(paramStartRow, paramStartColumn).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Range("V1:W1").Merge();

                paramStartRow++;
                worksheet.Cell(paramStartRow, paramStartColumn).Value = "URL:";
                worksheet.Cell(paramStartRow, paramStartColumn + 1).Value = url;

                string selectedMode = comboBoxMode.SelectedItem.ToString();

                switch (selectedMode)
                {
                    case "Stable":
                        paramStartRow++;
                        worksheet.Cell(paramStartRow, paramStartColumn).Value = "Number of Requests:";
                        worksheet.Cell(paramStartRow, paramStartColumn + 1).Value = totalRequests;
                        break;

                    default:
                        paramStartRow++;
                        worksheet.Cell(paramStartRow, paramStartColumn).Value = "Number of Requests (Min):";
                        worksheet.Cell(paramStartRow, paramStartColumn + 1).Value = totalRequests;

                        paramStartRow++;
                        worksheet.Cell(paramStartRow, paramStartColumn).Value = "Number of Requests (Max):";
                        worksheet.Cell(paramStartRow, paramStartColumn + 1).Value = maxRequests;
                        break;
                }

                paramStartRow++;
                worksheet.Cell(paramStartRow, paramStartColumn).Value = "Mode:";
                worksheet.Cell(paramStartRow, paramStartColumn + 1).Value = selectedMode;

                paramStartRow++;
                worksheet.Cell(paramStartRow, paramStartColumn).Value = "Timeout Per Round:";
                worksheet.Cell(paramStartRow, paramStartColumn + 1).Value = timeoutInSeconds + " second(s)";

                paramStartRow++;
                worksheet.Cell(paramStartRow, paramStartColumn).Value = "Time in Period:";
                worksheet.Cell(paramStartRow, paramStartColumn + 1).Value = durationTimePeriod + " " + selectedTimePeriod;

                worksheet.Columns().AdjustToContents();

                using (var saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = "Excel Files|*.xlsx";
                    saveFileDialog.Title = "Save an Excel File";
                    saveFileDialog.FileName = "EnduranceTestResults.xlsx";

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        workbook.SaveAs(saveFileDialog.FileName);
                        MessageBox.Show("Export successful!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }

        private void ExportToCsv(string url)
        {
            using (var saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "CSV Files|*.csv";
                saveFileDialog.Title = "Save CSV File";
                saveFileDialog.FileName = "EnduranceTestResults.csv";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (StreamWriter writer = new StreamWriter(saveFileDialog.FileName))
                        {
                            writer.WriteLine("Round,Status,Reason,LoadTime(ms),WaitTime(ms),ResponseTime(ms),CPUUsage(%),RAMUsage(MB)," +
                                             "TotalRequests,SuccessfulRequests,FailedRequests,AverageLoadTime(ms),AverageWaitTime(ms)," +
                                             "AverageResponseTime(ms),Throughput(req/s),ErrorRate(%),RoundDuration(s)");

                            foreach (var result in enduranceTestResults)
                            {
                                writer.WriteLine($"{result.Round},{(int)result.StatusCode},\"{result.ReasonPhrase}\"," +
                                                $"{result.LoadTime.TotalMilliseconds},{result.WaitTime.TotalMilliseconds},{result.ResponseTime.TotalMilliseconds}," +
                                                $"{result.CpuUsage},{result.RamUsage}," +
                                                $"{result.RequestPerRound},{result.SuccessfulRequests},{result.FailedRequests}," +
                                                $"{result.AverageLoadTime},{result.AverageWaitTime},{result.AverageResponseTime}," +
                                                $"{result.Throughput},{result.ErrorRate},{result.RoundDuration}");
                            }

                            writer.WriteLine();

                            double averageCpuUsage = currentRound > 0 ? totalCpuUsage / currentRound : 0;
                            double averageRamUsage = currentRound > 0 ? totalRamUsage / currentRound : 0;
                            double averageLoadTime = totalResponses > 0 ? totalLoadTime / totalResponses : 0;
                            double averageWaitTime = totalResponses > 0 ? totalWaitTime / totalResponses : 0;
                            double averageResponseTime = totalResponses > 0 ? totalResponseTime / totalResponses : 0;
                            double averageThroughput = totalResponses > 0 ? totalThroughput / currentRound : 0;
                            double averageErrorRate = totalRequestsProcessed > 0 ? (double)totalFailedRequests / totalRequestsProcessed * 100 : 0;
                            double averageRoundDuration = currentRound > 0 ? enduranceTestResults.Average(r => r.RoundDuration) : 0;

                            writer.WriteLine("SUMMARY");
                            writer.WriteLine($"URL,{url}");
                            writer.WriteLine($"Total Requests,{totalRequestsProcessed}");
                            writer.WriteLine($"Successful Requests,{totalSuccessfulRequests}");
                            writer.WriteLine($"Failed Requests,{totalFailedRequests}");
                            writer.WriteLine($"Average CPU Usage,{averageCpuUsage}%");
                            writer.WriteLine($"Average RAM Usage,{averageRamUsage} MB");
                            writer.WriteLine($"Average Load Time,{averageLoadTime} ms");
                            writer.WriteLine($"Average Wait Time,{averageWaitTime} ms");
                            writer.WriteLine($"Average Response Time,{averageResponseTime} ms");
                            writer.WriteLine($"Average Throughput,{averageThroughput} requests/second");
                            writer.WriteLine($"Average Error Rate,{averageErrorRate}%");
                            writer.WriteLine($"Average Round Duration,{averageRoundDuration} seconds");
                        }

                        MessageBox.Show("Successfully export to CSV!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Failed export to CSV: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void ExportToJson(string url)
        {
            using (var saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "JSON Files|*.json";
                saveFileDialog.Title = "Save JSON File";
                saveFileDialog.FileName = "EnduranceTestResults.json";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var testSummary = new
                        {
                            TestInfo = new
                            {
                                TargetUrl = url,
                                TestDate = DateTime.Now.ToString("dd MMMM yyyy HH:mm:ss", new CultureInfo("en-US")),
                                TotalRounds = currentRound,
                                TotalRequests = totalRequestsProcessed,
                                SuccessfulRequests = totalSuccessfulRequests,
                                FailedRequests = totalFailedRequests,
                                TestDuration = durationInSeconds,
                                TestMode = comboBoxMode.SelectedItem.ToString()
                            },
                            Summary = new
                            {
                                AverageCpuUsage = currentRound > 0 ? totalCpuUsage / currentRound : 0,
                                AverageRamUsage = currentRound > 0 ? totalRamUsage / currentRound : 0,
                                AverageLoadTime = totalResponses > 0 ? totalLoadTime / totalResponses : 0,
                                AverageWaitTime = totalResponses > 0 ? totalWaitTime / totalResponses : 0,
                                AverageResponseTime = totalResponses > 0 ? totalResponseTime / totalResponses : 0,
                                AverageThroughput = currentRound > 0 ? totalThroughput / currentRound : 0,
                                AverageErrorRate = totalRequestsProcessed > 0 ? (double)totalFailedRequests / totalRequestsProcessed * 100 : 0,
                                AverageRoundDuration = currentRound > 0 ? enduranceTestResults.Average(r => r.RoundDuration) : 0
                            },
                            RoundSummaries = enduranceTestResults
                                .GroupBy(r => r.Round)
                                .Select(g => new
                                {
                                    Round = g.Key,
                                    TotalRequests = g.Count(),
                                    SuccessfulRequests = g.Count(r => r.StatusCode == System.Net.HttpStatusCode.OK),
                                    FailedRequests = g.Count(r => r.StatusCode != System.Net.HttpStatusCode.OK),
                                    AverageLoadTime = g.Average(r => r.LoadTime.TotalMilliseconds),
                                    AverageWaitTime = g.Average(r => r.WaitTime.TotalMilliseconds),
                                    AverageResponseTime = g.Average(r => r.ResponseTime.TotalMilliseconds),
                                    Throughput = g.Average(r => r.Throughput),
                                    ErrorRate = g.Average(r => r.ErrorRate),
                                    CpuUsage = g.Average(r => r.CpuUsage),
                                    RamUsage = g.Average(r => r.RamUsage)
                                }).ToList(),
                            DetailResults = enduranceTestResults.Select(r => new
                            {
                                Round = r.Round,
                                StatusCode = (int)r.StatusCode,
                                ReasonPhrase = r.ReasonPhrase,
                                LoadTimeMs = r.LoadTime.TotalMilliseconds,
                                WaitTimeMs = r.WaitTime.TotalMilliseconds,
                                ResponseTimeMs = r.ResponseTime.TotalMilliseconds,
                                CpuUsage = r.CpuUsage,
                                RamUsage = r.RamUsage,
                                Throughput = r.Throughput,
                                ErrorRate = r.ErrorRate,
                                RoundDuration = r.RoundDuration
                            }).ToList()
                        };

                        string jsonContent = System.Text.Json.JsonSerializer.Serialize(testSummary, new System.Text.Json.JsonSerializerOptions
                        {
                            WriteIndented = true
                        });

                        File.WriteAllText(saveFileDialog.FileName, jsonContent);

                        MessageBox.Show("Succeessfully export to JSON!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Failed export to JSON: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void ExportToHtml(string url)
        {
            using (var saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "HTML Files|*.html";
                saveFileDialog.Title = "Save HTML File";
                saveFileDialog.FileName = "EnduranceTestResults.html";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // Prepare summary data
                        double averageCpuUsage = currentRound > 0 ? totalCpuUsage / currentRound : 0;
                        double averageRamUsage = currentRound > 0 ? totalRamUsage / currentRound : 0;
                        double averageLoadTime = totalResponses > 0 ? totalLoadTime / totalResponses : 0;
                        double averageWaitTime = totalResponses > 0 ? totalWaitTime / totalResponses : 0;
                        double averageResponseTime = totalResponses > 0 ? totalResponseTime / totalResponses : 0;
                        double averageThroughput = currentRound > 0 ? totalThroughput / currentRound : 0;
                        double averageErrorRate = totalRequestsProcessed > 0 ? (double)totalFailedRequests / totalRequestsProcessed * 100 : 0;
                        double averageRoundDuration = currentRound > 0 ? enduranceTestResults.Average(r => r.RoundDuration) : 0;

                        StringBuilder htmlBuilder = new StringBuilder();

                        // HTML Document
                        htmlBuilder.AppendLine("<!DOCTYPE html>");
                        htmlBuilder.AppendLine("<html lang='id'>");
                        htmlBuilder.AppendLine("<head>");
                        htmlBuilder.AppendLine("<meta charset='UTF-8'>");
                        htmlBuilder.AppendLine("<meta name='viewport' content='width=device-width, initial-scale=1.0'>");
                        htmlBuilder.AppendLine("<title>Endurance Testing Report</title>");
                        htmlBuilder.AppendLine("<style>");
                        htmlBuilder.AppendLine("body { font-family: Arial, sans-serif; margin: 0; padding: 20px; }");
                        htmlBuilder.AppendLine(".container { max-width: 1200px; margin: 0 auto; }");
                        htmlBuilder.AppendLine("h1, h2 { color: #333; }");
                        htmlBuilder.AppendLine("table { width: 100%; border-collapse: collapse; margin: 20px 0; }");
                        htmlBuilder.AppendLine("th, td { padding: 10px; text-align: left; border: 1px solid #ddd; }");
                        htmlBuilder.AppendLine("th { background-color: #f2f2f2; }");
                        htmlBuilder.AppendLine("tr:nth-child(even) { background-color: #f9f9f9; }");
                        htmlBuilder.AppendLine(".summary-card { background-color: #f0f8ff; border-radius: 5px; padding: 15px; margin-bottom: 20px; }");
                        htmlBuilder.AppendLine(".summary-grid { display: grid; grid-template-columns: repeat(3, 1fr); gap: 15px; }");
                        htmlBuilder.AppendLine(".metric { background-color: white; padding: 10px; border-radius: 5px; box-shadow: 0 2px 4px rgba(0,0,0,0.1); }");
                        htmlBuilder.AppendLine(".metric h3 { margin-top: 0; color: #555; }");
                        htmlBuilder.AppendLine(".metric p { font-size: 20px; font-weight: bold; margin: 0; }");
                        htmlBuilder.AppendLine(".success { color: green; }");
                        htmlBuilder.AppendLine(".error { color: red; }");
                        htmlBuilder.AppendLine(".chart-container { height: 300px; margin: 20px 0; background: #f9f9f9; border-radius: 5px; padding: 15px; }");
                        htmlBuilder.AppendLine(".footer { margin-top: 30px; text-align: center; color: #777; font-size: 12px; }");
                        htmlBuilder.AppendLine(".tab { overflow: hidden; border: 1px solid #ccc; background-color: #f1f1f1; }");
                        htmlBuilder.AppendLine(".tab button { background-color: inherit; float: left; border: none; outline: none; cursor: pointer; padding: 14px 16px; transition: 0.3s; }");
                        htmlBuilder.AppendLine(".tab button:hover { background-color: #ddd; }");
                        htmlBuilder.AppendLine(".tab button.active { background-color: #ccc; }");
                        htmlBuilder.AppendLine(".tabcontent { display: none; padding: 6px 12px; border: 1px solid #ccc; border-top: none; }");
                        htmlBuilder.AppendLine(".tabcontent.active { display: block; }");
                        htmlBuilder.AppendLine("</style>");
                        htmlBuilder.AppendLine("<script src='https://cdn.jsdelivr.net/npm/chart.js'></script>");
                        htmlBuilder.AppendLine("</head>");
                        htmlBuilder.AppendLine("<body>");
                        htmlBuilder.AppendLine("<div class='container'>");

                        // Header
                        htmlBuilder.AppendLine("<h1>Endurance Testing Report</h1>");
                        htmlBuilder.AppendLine($"<p><strong>URL:</strong> {url}</p>");
                        htmlBuilder.AppendLine($"<p><strong>Test Date:</strong> {DateTime.Now.ToString("dd MMMM yyyy HH:mm:ss", new CultureInfo("en-US"))}</p>");
                        htmlBuilder.AppendLine($"<p><strong>Test Mode:</strong> {comboBoxMode.SelectedItem}</p>");
                        htmlBuilder.AppendLine($"<p><strong>Total Duration:</strong> {durationInSeconds} seconds</p>");

                        // Tabs for different sections
                        htmlBuilder.AppendLine("<div class='tab'>");
                        htmlBuilder.AppendLine("<button class='tablinks active' onclick='openTab(event, \"Summary\")'>Summary</button>");
                        htmlBuilder.AppendLine("<button class='tablinks' onclick='openTab(event, \"Charts\")'>Graph</button>");
                        htmlBuilder.AppendLine("<button class='tablinks' onclick='openTab(event, \"RoundData\")'>Data Per Round</button>");
                        htmlBuilder.AppendLine("<button class='tablinks' onclick='openTab(event, \"DetailData\")'>Data Detail</button>");
                        htmlBuilder.AppendLine("</div>");

                        // Summary Tab
                        htmlBuilder.AppendLine("<div id='Summary' class='tabcontent active'>");
                        htmlBuilder.AppendLine("<h2>Test Summary</h2>");
                        htmlBuilder.AppendLine("<div class='summary-card'>");
                        htmlBuilder.AppendLine("<div class='summary-grid'>");

                        // Request metrics
                        htmlBuilder.AppendLine("<div class='metric'>");
                        htmlBuilder.AppendLine("<h3>Total Requests</h3>");
                        htmlBuilder.AppendLine($"<p>{totalRequestsProcessed}</p>");
                        htmlBuilder.AppendLine("</div>");

                        htmlBuilder.AppendLine("<div class='metric'>");
                        htmlBuilder.AppendLine("<h3>Successfull Requests</h3>");
                        htmlBuilder.AppendLine($"<p class='success'>{totalSuccessfulRequests}</p>");
                        htmlBuilder.AppendLine("</div>");

                        htmlBuilder.AppendLine("<div class='metric'>");
                        htmlBuilder.AppendLine("<h3>Failed Requests</h3>");
                        htmlBuilder.AppendLine($"<p class='error'>{totalFailedRequests}</p>");
                        htmlBuilder.AppendLine("</div>");

                        // Performance metrics

                        htmlBuilder.AppendLine("<div class='metric'>");
                        htmlBuilder.AppendLine("<h3>Average Load Time</h3>");
                        htmlBuilder.AppendLine($"<p>{averageLoadTime:F2} ms</p>");
                        htmlBuilder.AppendLine("</div>");

                        htmlBuilder.AppendLine("<div class='metric'>");
                        htmlBuilder.AppendLine("<h3>Average Wait Time</h3>");
                        htmlBuilder.AppendLine($"<p>{averageWaitTime:F2} ms</p>");
                        htmlBuilder.AppendLine("</div>");

                        htmlBuilder.AppendLine("<div class='metric'>");
                        htmlBuilder.AppendLine("<h3>Average Response Time</h3>");
                        htmlBuilder.AppendLine($"<p>{averageResponseTime:F2} ms</p>");
                        htmlBuilder.AppendLine("</div>");

                        htmlBuilder.AppendLine("<div class='metric'>");
                        htmlBuilder.AppendLine("<h3>Average Throughput</h3>");
                        htmlBuilder.AppendLine($"<p>{averageThroughput:F2} req/s</p>");
                        htmlBuilder.AppendLine("</div>");

                        // Resource metrics
                        htmlBuilder.AppendLine("<div class='metric'>");
                        htmlBuilder.AppendLine("<h3>Average CPU Usage</h3>");
                        htmlBuilder.AppendLine($"<p>{averageCpuUsage:F2}%</p>");
                        htmlBuilder.AppendLine("</div>");

                        htmlBuilder.AppendLine("<div class='metric'>");
                        htmlBuilder.AppendLine("<h3>Average RAM Usage</h3>");
                        htmlBuilder.AppendLine($"<p>{averageRamUsage:F2} MB</p>");
                        htmlBuilder.AppendLine("</div>");

                        htmlBuilder.AppendLine("<div class='metric'>");
                        htmlBuilder.AppendLine("<h3>Average Error Rate</h3>");
                        htmlBuilder.AppendLine($"<p>{averageErrorRate:F2}%</p>");
                        htmlBuilder.AppendLine("</div>");

                        htmlBuilder.AppendLine("</div>"); // end summary-grid
                        htmlBuilder.AppendLine("</div>"); // end summary-card
                        htmlBuilder.AppendLine("</div>"); // end Summary tab

                        // Charts Tab
                        htmlBuilder.AppendLine("<div id='Charts' class='tabcontent'>");
                        htmlBuilder.AppendLine("<h2>Data Visualization</h2>");

                        // Load Time Chart
                        htmlBuilder.AppendLine("<h3>Load Time per Round</h3>");
                        htmlBuilder.AppendLine("<div class='chart-container'>");
                        htmlBuilder.AppendLine("<canvas id='loadTimeChart'></canvas>");
                        htmlBuilder.AppendLine("</div>");

                        // Wait Time Chart
                        htmlBuilder.AppendLine("<h3>Wait Time per Round</h3>");
                        htmlBuilder.AppendLine("<div class='chart-container'>");
                        htmlBuilder.AppendLine("<canvas id='waitTimeChart'></canvas>");
                        htmlBuilder.AppendLine("</div>");

                        // Response Time Chart
                        htmlBuilder.AppendLine("<h3>Response Time per Round</h3>");
                        htmlBuilder.AppendLine("<div class='chart-container'>");
                        htmlBuilder.AppendLine("<canvas id='responseTimeChart'></canvas>");
                        htmlBuilder.AppendLine("</div>");

                        // Throughput Chart
                        htmlBuilder.AppendLine("<h3>Throughput per Round</h3>");
                        htmlBuilder.AppendLine("<div class='chart-container'>");
                        htmlBuilder.AppendLine("<canvas id='throughputChart'></canvas>");
                        htmlBuilder.AppendLine("</div>");

                        // Error Rate Chart
                        htmlBuilder.AppendLine("<h3>Error Rate per Round</h3>");
                        htmlBuilder.AppendLine("<div class='chart-container'>");
                        htmlBuilder.AppendLine("<canvas id='errorRateChart'></canvas>");
                        htmlBuilder.AppendLine("</div>");
                        htmlBuilder.AppendLine("</div>"); // end Charts tab

                        // Round Data Tab
                        htmlBuilder.AppendLine("<div id='RoundData' class='tabcontent'>");
                        htmlBuilder.AppendLine("<h2>Data Per Round</h2>");
                        htmlBuilder.AppendLine("<table>");
                        htmlBuilder.AppendLine("<tr>");
                        htmlBuilder.AppendLine("<th>Ronde</th>");
                        htmlBuilder.AppendLine("<th>Total Requests</th>");
                        htmlBuilder.AppendLine("<th>Successful</th>");
                        htmlBuilder.AppendLine("<th>Failed</th>");
                        htmlBuilder.AppendLine("<th>Avg Load Time</th>");
                        htmlBuilder.AppendLine("<th>Avg Wait Time</th>");
                        htmlBuilder.AppendLine("<th>Avg Response Time</th>");
                        htmlBuilder.AppendLine("<th>Throughput</th>");
                        htmlBuilder.AppendLine("<th>Error Rate</th>");
                        htmlBuilder.AppendLine("<th>CPU Usage</th>");
                        htmlBuilder.AppendLine("<th>RAM Usage</th>");
                        htmlBuilder.AppendLine("</tr>");

                        // Group by round
                        var roundData = enduranceTestResults
                            .GroupBy(r => r.Round)
                            .Select(g => new
                            {
                                Round = g.Key,
                                TotalRequests = g.Count(),
                                SuccessfulRequests = g.Count(r => r.StatusCode == System.Net.HttpStatusCode.OK),
                                FailedRequests = g.Count(r => r.StatusCode != System.Net.HttpStatusCode.OK),
                                AvgLoadTime = g.Average(r => r.LoadTime.TotalMilliseconds),
                                AvgWaitTime = g.Average(r => r.WaitTime.TotalMilliseconds),
                                AvgResponseTime = g.Average(r => r.ResponseTime.TotalMilliseconds),
                                AvgThroughput = g.Average(r => r.Throughput),
                                AvgErrorRate = g.Average(r => r.ErrorRate),
                                AvgCpuUsage = g.Average(r => r.CpuUsage),
                                AvgRamUsage = g.Average(r => r.RamUsage)
                            })
                            .OrderBy(r => r.Round);

                        foreach (var round in roundData)
                        {
                            htmlBuilder.AppendLine("<tr>");
                            htmlBuilder.AppendLine($"<td>{round.Round}</td>");
                            htmlBuilder.AppendLine($"<td>{round.TotalRequests}</td>");
                            htmlBuilder.AppendLine($"<td>{round.SuccessfulRequests}</td>");
                            htmlBuilder.AppendLine($"<td>{round.FailedRequests}</td>");
                            htmlBuilder.AppendLine($"<td>{round.AvgLoadTime:F2} ms</td>");
                            htmlBuilder.AppendLine($"<td>{round.AvgWaitTime:F2} ms</td>");
                            htmlBuilder.AppendLine($"<td>{round.AvgResponseTime:F2} ms</td>");
                            htmlBuilder.AppendLine($"<td>{round.AvgThroughput:F2} req/s</td>");
                            htmlBuilder.AppendLine($"<td>{round.AvgErrorRate:F2}%</td>");
                            htmlBuilder.AppendLine($"<td>{round.AvgCpuUsage:F2}%</td>");
                            htmlBuilder.AppendLine($"<td>{round.AvgRamUsage:F2} MB</td>");
                            htmlBuilder.AppendLine("</tr>");
                        }

                        htmlBuilder.AppendLine("</table>");
                        htmlBuilder.AppendLine("</div>"); // end RoundData tab

                        // Detail Data Tab
                        htmlBuilder.AppendLine("<div id='DetailData' class='tabcontent'>");
                        htmlBuilder.AppendLine("<h2>Data Detail</h2>");
                        htmlBuilder.AppendLine("<div style='overflow-x:auto;'>");
                        htmlBuilder.AppendLine("<table>");
                        htmlBuilder.AppendLine("<tr>");
                        htmlBuilder.AppendLine("<th>Round</th>");
                        htmlBuilder.AppendLine("<th>Status</th>");
                        htmlBuilder.AppendLine("<th>Reason Phrase</th>");
                        htmlBuilder.AppendLine("<th>Load Time</th>");
                        htmlBuilder.AppendLine("<th>Wait Time</th>");
                        htmlBuilder.AppendLine("<th>Response Time</th>");
                        htmlBuilder.AppendLine("<th>CPU Usage</th>");
                        htmlBuilder.AppendLine("<th>RAM Usage</th>");
                        htmlBuilder.AppendLine("</tr>");

                        // Limit to 1000 rows for performance
                        foreach (var result in enduranceTestResults.Take(1000))
                        {
                            htmlBuilder.AppendLine("<tr>");
                            htmlBuilder.AppendLine($"<td>{result.Round}</td>");
                            htmlBuilder.AppendLine($"<td>{(int)result.StatusCode}</td>");
                            htmlBuilder.AppendLine($"<td>{result.ReasonPhrase}</td>");
                            htmlBuilder.AppendLine($"<td>{result.LoadTime.TotalMilliseconds:F2} ms</td>");
                            htmlBuilder.AppendLine($"<td>{result.WaitTime.TotalMilliseconds:F2} ms</td>");
                            htmlBuilder.AppendLine($"<td>{result.ResponseTime.TotalMilliseconds:F2} ms</td>");
                            htmlBuilder.AppendLine($"<td>{result.CpuUsage:F2}%</td>");
                            htmlBuilder.AppendLine($"<td>{result.RamUsage:F2} MB</td>");
                            htmlBuilder.AppendLine("</tr>");
                        }

                        htmlBuilder.AppendLine("</table>");
                        htmlBuilder.AppendLine("</div>");
                        htmlBuilder.AppendLine("</div>"); // end DetailData tab

                        // Footer
                        htmlBuilder.AppendLine("<div class='footer'>");
                        htmlBuilder.AppendLine($"<p>Made with Endurance Testing Tool by Rahma Bintang Pratama at {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}</p>");
                        htmlBuilder.AppendLine("</div>");

                        htmlBuilder.AppendLine("</div>"); // end container

                        // JavaScript for Charts
                        htmlBuilder.AppendLine("<script>");

                        // Data Preparation for Charts
                        htmlBuilder.AppendLine("document.addEventListener('DOMContentLoaded', function() {");

                        // Load Time Chart
                        htmlBuilder.AppendLine("var loadTimeData = {");
                        htmlBuilder.AppendLine("  labels: [" + string.Join(",", roundData.Select(r => r.Round)) + "],");
                        htmlBuilder.AppendLine("  datasets: [{");
                        htmlBuilder.AppendLine("    label: 'Load Time (ms)',");
                        htmlBuilder.AppendLine("    backgroundColor: 'rgba(54, 162, 235, 0.2)',");
                        htmlBuilder.AppendLine("    borderColor: 'rgba(54, 162, 235, 1)',");
                        htmlBuilder.AppendLine("    borderWidth: 1,");
                        htmlBuilder.AppendLine("    data: [" + string.Join(",", roundData.Select(r => r.AvgLoadTime.ToString("F2", System.Globalization.CultureInfo.InvariantCulture))) + "]");
                        htmlBuilder.AppendLine("  }]");
                        htmlBuilder.AppendLine("};");

                        htmlBuilder.AppendLine("var loadTimeCtx = document.getElementById('loadTimeChart').getContext('2d');");
                        htmlBuilder.AppendLine("var loadTimeChart = new Chart(loadTimeCtx, {");
                        htmlBuilder.AppendLine("  type: 'line',");
                        htmlBuilder.AppendLine("  data: loadTimeData,");
                        htmlBuilder.AppendLine("  options: {");
                        htmlBuilder.AppendLine("    responsive: true,");
                        htmlBuilder.AppendLine("    maintainAspectRatio: false,");
                        htmlBuilder.AppendLine("    scales: {");
                        htmlBuilder.AppendLine("      y: {");
                        htmlBuilder.AppendLine("        beginAtZero: true,");
                        htmlBuilder.AppendLine("        title: {");
                        htmlBuilder.AppendLine("          display: true,");
                        htmlBuilder.AppendLine("          text: 'Load Time (ms)'");
                        htmlBuilder.AppendLine("        }");
                        htmlBuilder.AppendLine("      },");
                        htmlBuilder.AppendLine("      x: {");
                        htmlBuilder.AppendLine("        title: {");
                        htmlBuilder.AppendLine("          display: true,");
                        htmlBuilder.AppendLine("          text: 'Round'");
                        htmlBuilder.AppendLine("        }");
                        htmlBuilder.AppendLine("      }");
                        htmlBuilder.AppendLine("    }");
                        htmlBuilder.AppendLine("  }");
                        htmlBuilder.AppendLine("});");

                        // Wait Time Chart
                        htmlBuilder.AppendLine("var waitTimeData = {");
                        htmlBuilder.AppendLine("  labels: [" + string.Join(",", roundData.Select(r => r.Round)) + "],");
                        htmlBuilder.AppendLine("  datasets: [{");
                        htmlBuilder.AppendLine("    label: 'Wait Time (ms)',");
                        htmlBuilder.AppendLine("    backgroundColor: 'rgba(54, 162, 235, 0.2)',");
                        htmlBuilder.AppendLine("    borderColor: 'rgba(54, 162, 235, 1)',");
                        htmlBuilder.AppendLine("    borderWidth: 1,");
                        htmlBuilder.AppendLine("    data: [" + string.Join(",", roundData.Select(r => r.AvgWaitTime.ToString("F2", System.Globalization.CultureInfo.InvariantCulture))) + "]");
                        htmlBuilder.AppendLine("  }]");
                        htmlBuilder.AppendLine("};");

                        htmlBuilder.AppendLine("var waitTimeCtx = document.getElementById('waitTimeChart').getContext('2d');");
                        htmlBuilder.AppendLine("var waitTimeChart = new Chart(waitTimeCtx, {");
                        htmlBuilder.AppendLine("  type: 'line',");
                        htmlBuilder.AppendLine("  data: waitTimeData,");
                        htmlBuilder.AppendLine("  options: {");
                        htmlBuilder.AppendLine("    responsive: true,");
                        htmlBuilder.AppendLine("    maintainAspectRatio: false,");
                        htmlBuilder.AppendLine("    scales: {");
                        htmlBuilder.AppendLine("      y: {");
                        htmlBuilder.AppendLine("        beginAtZero: true,");
                        htmlBuilder.AppendLine("        title: {");
                        htmlBuilder.AppendLine("          display: true,");
                        htmlBuilder.AppendLine("          text: 'Wait Time (ms)'");
                        htmlBuilder.AppendLine("        }");
                        htmlBuilder.AppendLine("      },");
                        htmlBuilder.AppendLine("      x: {");
                        htmlBuilder.AppendLine("        title: {");
                        htmlBuilder.AppendLine("          display: true,");
                        htmlBuilder.AppendLine("          text: 'Round'");
                        htmlBuilder.AppendLine("        }");
                        htmlBuilder.AppendLine("      }");
                        htmlBuilder.AppendLine("    }");
                        htmlBuilder.AppendLine("  }");
                        htmlBuilder.AppendLine("});");

                        // Response Time Chart
                        htmlBuilder.AppendLine("var responseTimeData = {");
                        htmlBuilder.AppendLine("  labels: [" + string.Join(",", roundData.Select(r => r.Round)) + "],");
                        htmlBuilder.AppendLine("  datasets: [{");
                        htmlBuilder.AppendLine("    label: 'Response Time (ms)',");
                        htmlBuilder.AppendLine("    backgroundColor: 'rgba(54, 162, 235, 0.2)',");
                        htmlBuilder.AppendLine("    borderColor: 'rgba(54, 162, 235, 1)',");
                        htmlBuilder.AppendLine("    borderWidth: 1,");
                        htmlBuilder.AppendLine("    data: [" + string.Join(",", roundData.Select(r => r.AvgResponseTime.ToString("F2", System.Globalization.CultureInfo.InvariantCulture))) + "]");
                        htmlBuilder.AppendLine("  }]");
                        htmlBuilder.AppendLine("};");

                        htmlBuilder.AppendLine("var responseTimeCtx = document.getElementById('responseTimeChart').getContext('2d');");
                        htmlBuilder.AppendLine("var responseTimeChart = new Chart(responseTimeCtx, {");
                        htmlBuilder.AppendLine("  type: 'line',");
                        htmlBuilder.AppendLine("  data: responseTimeData,");
                        htmlBuilder.AppendLine("  options: {");
                        htmlBuilder.AppendLine("    responsive: true,");
                        htmlBuilder.AppendLine("    maintainAspectRatio: false,");
                        htmlBuilder.AppendLine("    scales: {");
                        htmlBuilder.AppendLine("      y: {");
                        htmlBuilder.AppendLine("        beginAtZero: true,");
                        htmlBuilder.AppendLine("        title: {");
                        htmlBuilder.AppendLine("          display: true,");
                        htmlBuilder.AppendLine("          text: 'Response Time (ms)'");
                        htmlBuilder.AppendLine("        }");
                        htmlBuilder.AppendLine("      },");
                        htmlBuilder.AppendLine("      x: {");
                        htmlBuilder.AppendLine("        title: {");
                        htmlBuilder.AppendLine("          display: true,");
                        htmlBuilder.AppendLine("          text: 'Round'");
                        htmlBuilder.AppendLine("        }");
                        htmlBuilder.AppendLine("      }");
                        htmlBuilder.AppendLine("    }");
                        htmlBuilder.AppendLine("  }");
                        htmlBuilder.AppendLine("});");

                        // Throughput Chart
                        htmlBuilder.AppendLine("var throughputData = {");
                        htmlBuilder.AppendLine("  labels: [" + string.Join(",", roundData.Select(r => r.Round)) + "],");
                        htmlBuilder.AppendLine("  datasets: [{");
                        htmlBuilder.AppendLine("    label: 'Throughput (req/s)',");
                        htmlBuilder.AppendLine("    backgroundColor: 'rgba(75, 192, 192, 0.2)',");
                        htmlBuilder.AppendLine("    borderColor: 'rgba(75, 192, 192, 1)',");
                        htmlBuilder.AppendLine("    borderWidth: 1,");
                        htmlBuilder.AppendLine("    data: [" + string.Join(",", roundData.Select(r => r.AvgThroughput.ToString("F2", System.Globalization.CultureInfo.InvariantCulture))) + "]");
                        htmlBuilder.AppendLine("  }]");
                        htmlBuilder.AppendLine("};");

                        htmlBuilder.AppendLine("var throughputCtx = document.getElementById('throughputChart').getContext('2d');");
                        htmlBuilder.AppendLine("var throughputChart = new Chart(throughputCtx, {");
                        htmlBuilder.AppendLine("  type: 'line',");
                        htmlBuilder.AppendLine("  data: throughputData,");
                        htmlBuilder.AppendLine("  options: {");
                        htmlBuilder.AppendLine("    responsive: true,");
                        htmlBuilder.AppendLine("    maintainAspectRatio: false,");
                        htmlBuilder.AppendLine("    scales: {");
                        htmlBuilder.AppendLine("      y: {");
                        htmlBuilder.AppendLine("        beginAtZero: true,");
                        htmlBuilder.AppendLine("        title: {");
                        htmlBuilder.AppendLine("          display: true,");
                        htmlBuilder.AppendLine("          text: 'Throughput (req/s)'");
                        htmlBuilder.AppendLine("        }");
                        htmlBuilder.AppendLine("      },");
                        htmlBuilder.AppendLine("      x: {");
                        htmlBuilder.AppendLine("        title: {");
                        htmlBuilder.AppendLine("          display: true,");
                        htmlBuilder.AppendLine("          text: 'Round'");
                        htmlBuilder.AppendLine("        }");
                        htmlBuilder.AppendLine("      }");
                        htmlBuilder.AppendLine("    }");
                        htmlBuilder.AppendLine("  }");
                        htmlBuilder.AppendLine("});");

                        // Error Rate Chart
                        htmlBuilder.AppendLine("var errorRateData = {");
                        htmlBuilder.AppendLine("  labels: [" + string.Join(",", roundData.Select(r => r.Round)) + "],");
                        htmlBuilder.AppendLine("  datasets: [{");
                        htmlBuilder.AppendLine("    label: 'Error Rate (%)',");
                        htmlBuilder.AppendLine("    backgroundColor: 'rgba(255, 99, 132, 0.2)',");
                        htmlBuilder.AppendLine("    borderColor: 'rgba(255, 99, 132, 1)',");
                        htmlBuilder.AppendLine("    borderWidth: 1,");
                        htmlBuilder.AppendLine("    data: [" + string.Join(",", roundData.Select(r => r.AvgErrorRate.ToString("F2", System.Globalization.CultureInfo.InvariantCulture))) + "]");
                        htmlBuilder.AppendLine("  }]");
                        htmlBuilder.AppendLine("};");

                        htmlBuilder.AppendLine("var errorRateCtx = document.getElementById('errorRateChart').getContext('2d');");
                        htmlBuilder.AppendLine("var errorRateChart = new Chart(errorRateCtx, {");
                        htmlBuilder.AppendLine("  type: 'line',");
                        htmlBuilder.AppendLine("  data: errorRateData,");
                        htmlBuilder.AppendLine("  options: {");
                        htmlBuilder.AppendLine("    responsive: true,");
                        htmlBuilder.AppendLine("    maintainAspectRatio: false,");
                        htmlBuilder.AppendLine("    scales: {");
                        htmlBuilder.AppendLine("      y: {");
                        htmlBuilder.AppendLine("        beginAtZero: true,");
                        htmlBuilder.AppendLine("        title: {");
                        htmlBuilder.AppendLine("          display: true,");
                        htmlBuilder.AppendLine("          text: 'Error Rate (%)'");
                        htmlBuilder.AppendLine("        }");
                        htmlBuilder.AppendLine("      },");
                        htmlBuilder.AppendLine("      x: {");
                        htmlBuilder.AppendLine("        title: {");
                        htmlBuilder.AppendLine("          display: true,");
                        htmlBuilder.AppendLine("          text: 'Round'");
                        htmlBuilder.AppendLine("        }");
                        htmlBuilder.AppendLine("      }");
                        htmlBuilder.AppendLine("    }");
                        htmlBuilder.AppendLine("  }");
                        htmlBuilder.AppendLine("});");

                        htmlBuilder.AppendLine("});"); // DOMContentLoaded event end

                        // Tab Functionality
                        htmlBuilder.AppendLine("function openTab(evt, tabName) {");
                        htmlBuilder.AppendLine("  var i, tabcontent, tablinks;");
                        htmlBuilder.AppendLine("  tabcontent = document.getElementsByClassName('tabcontent');");
                        htmlBuilder.AppendLine("  for (i = 0; i < tabcontent.length; i++) {");
                        htmlBuilder.AppendLine("    tabcontent[i].style.display = 'none';");
                        htmlBuilder.AppendLine("    tabcontent[i].className = tabcontent[i].className.replace(' active', '');");
                        htmlBuilder.AppendLine("  }");
                        htmlBuilder.AppendLine("  tablinks = document.getElementsByClassName('tablinks');");
                        htmlBuilder.AppendLine("  for (i = 0; i < tablinks.length; i++) {");
                        htmlBuilder.AppendLine("    tablinks[i].className = tablinks[i].className.replace(' active', '');");
                        htmlBuilder.AppendLine("  }");
                        htmlBuilder.AppendLine("  document.getElementById(tabName).style.display = 'block';");
                        htmlBuilder.AppendLine("  document.getElementById(tabName).className += ' active';");
                        htmlBuilder.AppendLine("  evt.currentTarget.className += ' active';");
                        htmlBuilder.AppendLine("}");

                        htmlBuilder.AppendLine("</script>");
                        htmlBuilder.AppendLine("</body>");
                        htmlBuilder.AppendLine("</html>");

                        // Write to file
                        File.WriteAllText(saveFileDialog.FileName, htmlBuilder.ToString());

                        MessageBox.Show("Successfully export to HTML!",
                            "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Failed export to HTML: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }

        public class EnduranceTestResult
    {
        public System.Net.HttpStatusCode StatusCode { get; set; }
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
