using System;
using System.Collections.Generic;
using ClosedXML.Excel;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text;
using System.Linq;
using System.Net.Sockets;

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
            totalResponseTime = 0;
            totalResponses = 0;
            totalRequestsProcessed = 0;
            totalErrors = 0;
            totalThroughput = 0;

            var countdownTask = StartCountdown(durationInSeconds, cancellationTokenSource.Token);

            await RunEnduranceTest(url, cancellationTokenSource.Token);

            await countdownTask;

            isRunning = false;

            btnClear.Enabled = true;
            btnExport.Enabled = true;

            ShowSummary();
        }

        private async Task StartCountdown(long durationInSeconds, CancellationToken cancellationToken)
        {
            long remainingTime = durationInSeconds;

            while (remainingTime > 0 && !cancellationToken.IsCancellationRequested)
            {
                TimeSpan timeLeft = TimeSpan.FromSeconds(remainingTime);
                lblTimeLeft.Text = $"{(int)timeLeft.TotalDays}:{timeLeft.Hours:D2}:{timeLeft.Minutes:D2}:{timeLeft.Seconds:D2}";

                await Task.Delay(1000);
                remainingTime--;
            }

            lblTimeLeft.Text = "00:00:00:00";

            if (!cancellationToken.IsCancellationRequested)
            {
                cancellationTokenSource.Cancel();
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
                    double roundResponseTime = 0;

                    Stopwatch stopwatchRound = new Stopwatch();
                    stopwatchRound.Start();

                    switch (selectedMode)
                    {
                        case "Progressive":
                            double progress = stopwatchTotal.Elapsed.TotalSeconds / durationInSeconds;
                            currentRequests = (int)(totalRequests + (maxRequests - totalRequests) * progress);
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
                        double validResponseTimeSum = 0;
                        int validResponseCount = 0;

                        foreach (var result in results)
                        {
                            if (result.StatusCode == System.Net.HttpStatusCode.OK)
                            {
                                validResponseTimeSum += result.ResponseTime.TotalMilliseconds;
                                validResponseCount++;
                            }
                            else
                            {
                                validResponseTimeSum += result.ResponseTime.TotalMilliseconds;
                            }
                        }
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
                            result.AverageResponseTime = averageRoundResponseTime;
                            result.Throughput = throughput;
                            result.ErrorRate = errorRate;
                            result.RoundDuration = roundDuration;
                        }

                        totalCpuUsage += roundCpuUsage;
                        totalRamUsage += roundRamUsage;
                        totalResponseTime += validResponseTimeSum;
                        totalResponses += currentRequests;
                        totalRequestsProcessed += currentRequests;

                        DisplayRoundStatistics(roundCpuUsage,
                                               roundRamUsage,
                                               roundSuccessfulRequests,
                                               roundFailedRequests,
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
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            try
            {
                using (var requestTimeout = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutInSeconds)))
                {
                    using (var combinedToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, requestTimeout.Token))
                    {
                        HttpResponseMessage response = await httpClient.GetAsync(url, combinedToken.Token);
                        TimeSpan responseTime = stopwatch.Elapsed;

                        return new EnduranceTestResult
                        {
                            StatusCode = response.StatusCode,
                            ReasonPhrase = response.ReasonPhrase,
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
            catch (TaskCanceledException ex)
            {
                return new EnduranceTestResult
                {
                    StatusCode = System.Net.HttpStatusCode.RequestTimeout,
                    ReasonPhrase = "Request Timeout - The request was canceled due to timeout",
                    ResponseTime = stopwatch.Elapsed,
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
                    ResponseTime = stopwatch.Elapsed,
                    Round = round,
                    CpuUsage = 0,
                    RamUsage = 0,
                    SuccessfulRequests = 0,
                    FailedRequests = 1,
                    AverageResponseTime = (double)stopwatch.Elapsed.TotalMilliseconds,
                    RequestPerRound = currentRequests
                };
            }
            finally
            {
                stopwatch.Stop();
            }
        }

        private void DisplayResult(EnduranceTestResult result, int round)
        {
            string resultString = $"Round {round}: Status: {(int)result.StatusCode}, Reason: {result.ReasonPhrase}, Response Time: {result.ResponseTime.TotalMilliseconds} ms";
            textBoxOutput.AppendText(resultString + Environment.NewLine);
            textBoxOutput.ScrollToCaret();
        }

        private void DisplayRoundStatistics(double currentCpuUsage,
                                            double currentRamUsage,
                                            int roundSuccessfulRequests,
                                            int roundFailedRequests,
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

            double averageResponseTime = totalResponses > 0 ? totalResponseTime / totalResponses : 0;
            double averageThroughput = totalResponses > 0 ? totalThroughput / currentRound : 0;
            double averageRoundDuration = currentRound > 0 ? enduranceTestResults.Average(result => result.RoundDuration) : 0;

            string summaryMessage = $"Summary:{Environment.NewLine}" +
                                    $"Total Requests: {totalRequestsProcessed}{Environment.NewLine}" +
                                    $"Successful Requests: {totalSuccessfulRequests}{Environment.NewLine}" +
                                    $"Failed Requests: {totalFailedRequests}{Environment.NewLine}" +
                                    $"Average Computer's CPU Usage: {averageCpuUsage}%{Environment.NewLine}" +
                                    $"Average Computer's RAM Usage: {averageRamUsage} MB{Environment.NewLine}" +
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
            using (var ramCounter = new PerformanceCounter("Memory", "Available MBytes"))
            {
                return ramCounter.NextValue();
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
            ExportToExcel(url);
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
                worksheet.Range("A1:M1").Merge();

                worksheet.Cell(2, 1).Value = "Round";
                worksheet.Cell(2, 2).Value = "Status";
                worksheet.Cell(2, 3).Value = "Reason";
                worksheet.Cell(2, 4).Value = "Response Time (ms)";
                worksheet.Cell(2, 5).Value = "Computer's CPU Usage (%)";
                worksheet.Cell(2, 6).Value = "Computer's RAM Usage (MB)";
                worksheet.Cell(2, 7).Value = "Total Requests";
                worksheet.Cell(2, 8).Value = "Successful Requests";
                worksheet.Cell(2, 9).Value = "Failed Requests";
                worksheet.Cell(2, 10).Value = "Average Response Time (ms)";
                worksheet.Cell(2, 11).Value = "Throughput (requests/sec)";
                worksheet.Cell(2, 12).Value = "Error Rate (%)";
                worksheet.Cell(2, 13).Value = "Round Duration (seconds)";

                worksheet.SheetView.Freeze(2, 0);

                int row = 3;
                foreach (var result in enduranceTestResults)
                {
                    worksheet.Cell(row, 1).Value = result.Round;
                    worksheet.Cell(row, 2).Value = (int)result.StatusCode;
                    worksheet.Cell(row, 3).Value = result.ReasonPhrase;
                    worksheet.Cell(row, 4).Value = result.ResponseTime.TotalMilliseconds;
                    worksheet.Cell(row, 5).Value = result.CpuUsage;
                    worksheet.Cell(row, 6).Value = result.RamUsage;
                    worksheet.Cell(row, 7).Value = result.RequestPerRound;
                    worksheet.Cell(row, 8).Value = result.SuccessfulRequests;
                    worksheet.Cell(row, 9).Value = result.FailedRequests;
                    worksheet.Cell(row, 10).Value = result.AverageResponseTime;
                    worksheet.Cell(row, 11).Value = result.Throughput;
                    worksheet.Cell(row, 12).Value = result.ErrorRate;
                    worksheet.Cell(row, 13).Value = result.RoundDuration;
                    row++;
                }

                int summaryStartRow = 1;
                int summaryStartColumn = 15;

                worksheet.Cell(summaryStartRow, summaryStartColumn).Value = "Overall Summary";
                worksheet.Cell(summaryStartRow, summaryStartColumn).Style.Font.Bold = true;
                worksheet.Cell(summaryStartRow, summaryStartColumn).Style.Font.FontSize = 14;
                worksheet.Cell(summaryStartRow, summaryStartColumn).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Range("O1:P1").Merge();


                double averageCpuUsageOverall = currentRound > 0 ? totalCpuUsage / currentRound : 0;
                double averageRamUsageOverall = currentRound > 0 ? totalRamUsage / currentRound : 0;
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
                int paramStartColumn = 18;

                worksheet.Cell(paramStartRow, paramStartColumn).Value = "Test Parameter";
                worksheet.Cell(paramStartRow, paramStartColumn).Style.Font.Bold = true;
                worksheet.Cell(paramStartRow, paramStartColumn).Style.Font.FontSize = 14;
                worksheet.Cell(paramStartRow, paramStartColumn).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Range("R1:S1").Merge();

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
    }

        public class EnduranceTestResult
    {
        public System.Net.HttpStatusCode StatusCode { get; set; }
        public string ReasonPhrase { get; set; }
        public TimeSpan ResponseTime { get; set; }
        public int Round { get; set; }
        public double CpuUsage { get; set; }
        public double RamUsage { get; set; }
        public int RequestPerRound { get; set; }
        public int SuccessfulRequests { get; set; }
        public int FailedRequests { get; set; }
        public double AverageResponseTime { get; set; }
        public double Throughput { get; set; }
        public double ErrorRate { get; set; }
        public double RoundDuration { get; set; }
    }
}
