using System;
using System.Collections.Generic;
using ClosedXML.Excel;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text;

namespace Endurance_Testing
{
    public partial class EnduranceTesting : Form
    {
        private CancellationTokenSource cancellationTokenSource;
        private List<EnduranceTestResult> enduranceTestResults = new List<EnduranceTestResult>();
        private int totalRequests;
        private int timeoutInSeconds;
        private long durationInSeconds;
        private int currentRound;
        private int totalSuccessfulRequests;
        private int totalFailedRequests;
        private int totalErrors;
        private float totalCpuUsage;
        private float totalRamUsage;
        private float totalResponseTime;
        private int totalResponses;
        private float totalThroughput;
        private bool isRunning = false;
        private string selectedTimePeriod;

        public EnduranceTesting()
        {
            InitializeComponent();
            this.Load += EnduranceTesting_Load;
            this.FormClosing += EnduranceTesting_FormClosing;
            textBoxInputRequest.KeyPress += new KeyPressEventHandler(textBoxOnlyNumber_KeyPress);
            textBoxTimeout.KeyPress += new KeyPressEventHandler(textBoxOnlyNumber_KeyPress);
            textBoxTime.KeyPress += new KeyPressEventHandler(textBoxOnlyNumber_KeyPress);
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

                while (!cancellationToken.IsCancellationRequested)
                {
                    currentRound++;
                    int roundSuccessfulRequests = 0;
                    int roundFailedRequests = 0;
                    float roundResponseTime = 0;

                    Stopwatch stopwatchRound = new Stopwatch();
                    stopwatchRound.Start();

                    var tasks = new List<Task<EnduranceTestResult>>();
                    for (int i = 0; i < totalRequests; i++)
                    {
                        tasks.Add(SendHttpRequest(httpClient, url, currentRound, cancellationToken));
                    }

                    var results = await Task.WhenAll(tasks);
                    enduranceTestResults.AddRange(results);

                    float roundCpuUsage = GetCpuUsage();
                    float roundRamUsage = GetRamUsage();

                    foreach (var result in results)
                    {
                        DisplayResult(result, currentRound);
                        roundResponseTime += (float)result.ResponseTime.TotalMilliseconds;

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
                    double throughputDuration = Math.Min(roundDurationInSeconds, timeoutInSeconds);

                    if (totalRequests > 0)
                    {
                        float averageRoundResponseTime = roundResponseTime / totalRequests;
                        float throughput = (float)roundSuccessfulRequests / (throughputDuration > 0 ? (float)throughputDuration : 1);
                        totalThroughput += throughput;
                        float errorRate = (float)roundFailedRequests / (float)totalRequests * 100;

                        foreach (var result in results)
                        {
                            result.CpuUsage = roundCpuUsage;
                            result.RamUsage = roundRamUsage;
                            result.SuccessfulRequests = roundSuccessfulRequests;
                            result.FailedRequests = roundFailedRequests;
                            result.AverageResponseTime = averageRoundResponseTime;
                            result.Throughput = throughput;
                            result.ErrorRate = errorRate;
                        }

                        totalCpuUsage += roundCpuUsage;
                        totalRamUsage += roundRamUsage;
                        totalResponseTime += roundResponseTime;
                        totalResponses += totalRequests;

                        DisplayRoundStatistics(roundCpuUsage, roundRamUsage, roundSuccessfulRequests, roundFailedRequests, averageRoundResponseTime, throughput, errorRate);
                    }

                    if (stopwatchTotal.Elapsed.TotalSeconds >= durationInSeconds)
                    {
                        cancellationTokenSource.Cancel();
                        break;
                    }

                    await Task.Delay(timeoutInSeconds * 1000);
                }

                stopwatchTotal.Stop();
                btnStart.Enabled = true;
                btnStop.Enabled = false;
            }
        }

        private async Task<EnduranceTestResult> SendHttpRequest(HttpClient httpClient, string url, int round, CancellationToken cancellationToken)
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
                            AverageResponseTime = (float)responseTime.TotalMilliseconds
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
                    AverageResponseTime = 0
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
                    AverageResponseTime = (float)stopwatch.Elapsed.TotalMilliseconds
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

        private void DisplayRoundStatistics(float currentCpuUsage, float currentRamUsage, int roundSuccessfulRequests, int roundFailedRequests, float averageRoundResponseTime, float throughput, float errorRate)
        {
            string roundStats = $"Round {currentRound} Statistics:{Environment.NewLine}" +
                                $"Computer's CPU Usage: {currentCpuUsage}%{Environment.NewLine}" +
                                $"Computer's RAM Usage: {currentRamUsage} MB{Environment.NewLine}" +
                                $"Total Requests: {totalRequests}{Environment.NewLine}" +
                                $"Successful Requests: {roundSuccessfulRequests}{Environment.NewLine}" +
                                $"Failed Requests: {roundFailedRequests}{Environment.NewLine}" +
                                $"Average Response Time: {averageRoundResponseTime} ms{Environment.NewLine}" +
                                $"Throughput: {throughput} requests/second{Environment.NewLine}" +
                                $"Error Rate: {errorRate}%{Environment.NewLine}{Environment.NewLine}";

            textBoxOutput.AppendText(roundStats);
            textBoxOutput.ScrollToCaret();
        }

        private void ShowSummary()
        {
            float averageCpuUsage = currentRound > 0 ? totalCpuUsage / currentRound : 0;
            float averageRamUsage = currentRound > 0 ? totalRamUsage / currentRound : 0;

            float averageResponseTime = totalResponses > 0 ? totalResponseTime / totalResponses : 0;
            float averageThroughput = totalResponses > 0 ? totalThroughput / currentRound : 0;

            string summaryMessage = $"Summary:{Environment.NewLine}" +
                                    $"Total Requests: {totalRequests * currentRound}{Environment.NewLine}" +
                                    $"Successful Requests: {totalSuccessfulRequests}{Environment.NewLine}" +
                                    $"Failed Requests: {totalFailedRequests}{Environment.NewLine}" +
                                    $"Average Computer's CPU Usage: {averageCpuUsage}%{Environment.NewLine}" +
                                    $"Average Computer's RAM Usage: {averageRamUsage} MB{Environment.NewLine}" +
                                    $"Average Response Time: {averageResponseTime} ms{Environment.NewLine}" +
                                    $"Average Throughput: {averageThroughput} requests/second{Environment.NewLine}" +
                                    $"Error Rate: {(totalFailedRequests / (float)(totalRequests * currentRound)) * 100}%{Environment.NewLine}{Environment.NewLine}";

            textBoxOutput.AppendText(summaryMessage);
            textBoxOutput.ScrollToCaret();
        }

        private float GetCpuUsage()
        {
            using (var cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total"))
            {
                cpuCounter.NextValue();
                Thread.Sleep(1000);
                return cpuCounter.NextValue();
            }
        }

        private float GetRamUsage()
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

            helpMessage.AppendLine("User Guide for Endurance Testing:");
            helpMessage.AppendLine();
            helpMessage.AppendLine("1. Enter the target URL in the URL column.");
            helpMessage.AppendLine("2. Enter the number of requests in the Requests column (maximum 8 digits).");
            helpMessage.AppendLine("3. Enter the duration for the endurance test in the Time column (in seconds, minutes, or hours).");
            helpMessage.AppendLine("4. Select the time period (seconds, minutes, or hours) using the radio buttons.");
            helpMessage.AppendLine("5. Enter the timeout per round in the Timeout column (in seconds).");
            helpMessage.AppendLine("6. Click the 'Start' button to begin the endurance testing.");
            helpMessage.AppendLine("7. Monitor the results in the output area and time left in real-time.");
            helpMessage.AppendLine("8. After the test, the output will display:");
            helpMessage.AppendLine("   a. Total Requests.");
            helpMessage.AppendLine("   b. Successful Requests.");
            helpMessage.AppendLine("   c. Failed Requests.");
            helpMessage.AppendLine("   d. Average Computer's CPU Usage (in percentage).");
            helpMessage.AppendLine("   e. Average Computer's RAM Usage (in megabytes).");
            helpMessage.AppendLine("   f. Average Response Time (in milliseconds).");
            helpMessage.AppendLine("   g. Throughput (requests per second).");
            helpMessage.AppendLine("   h. Error Rate (in percentage).");
            helpMessage.AppendLine("   i. Round Duration (in seconds).");
            helpMessage.AppendLine("9. Optionally, export the endurance testing results to an Excel file using the 'Export' button.");
            helpMessage.AppendLine("10.Click the 'Clear' button to reset the input fields and output area.");
            helpMessage.AppendLine();
            helpMessage.AppendLine("Note:");
            helpMessage.AppendLine("1. Ensure that your internet connection is stable and reliable for conducting this test.");
            helpMessage.AppendLine("2. Be aware that the device may become slower during the testing process, so ensure that your device specifications are adequate.");
            helpMessage.AppendLine("3. Additionally, the total duration of the test may not match the input time in the Time column due to processing time required for handling requests and responses.");

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

            infoMessage.AppendLine("Endurance Testing:");
            infoMessage.AppendLine();
            infoMessage.AppendLine("\"Endurance testing, also known as soak testing, involves subjecting an application to a sustained load for an extended period. This methodology helps uncover memory leaks, resource depletion, and other performance degradation issues that might only surface after prolonged usage.\"[1]");
            infoMessage.AppendLine("[1] S. Pargaonkar, \"A Comprehensive Review of Performance Testing Methodologies and Best Practices: Software Quality Engineering,\" International Journal of Science and Research (IJSR), vol. 12, no. 8, pp. 2008-2014, August 2023.");
            infoMessage.AppendLine();
            infoMessage.AppendLine("Metrics:");
            infoMessage.AppendLine();
            infoMessage.AppendLine("1. CPU Usage:");
            infoMessage.AppendLine("    a. Description:");
            infoMessage.AppendLine("        The CPU usage metric represents the percentage of the computer's processing power utilized during the endurance testing.");
            infoMessage.AppendLine("    b. Formula:");
            infoMessage.AppendLine("        CPU Usage = Current CPU Usage");
            infoMessage.AppendLine("2. RAM Usage:");
            infoMessage.AppendLine("    a. Description:");
            infoMessage.AppendLine("        The RAM usage metric indicates the amount of computer memory used during the endurance testing.");
            infoMessage.AppendLine("    b. Formula:");
            infoMessage.AppendLine("        RAM Usage = Current RAM Usage");
            infoMessage.AppendLine("3. Total Requests:");
            infoMessage.AppendLine("    a. Description:");
            infoMessage.AppendLine("        Total Requests is the sum of all requests sent during the endurance testing.");
            infoMessage.AppendLine("    b. Formula:");
            infoMessage.AppendLine("        Total Requests = Number of Requests Sent");
            infoMessage.AppendLine("4. Successful Requests:");
            infoMessage.AppendLine("    a. Description:");
            infoMessage.AppendLine("        Successful Requests is the sum of requests with an HTTP status code of 200 (OK).");
            infoMessage.AppendLine("    b. Formula:");
            infoMessage.AppendLine("        Successful Requests = Number of Requests with HTTP Status Code 200 (OK)");
            infoMessage.AppendLine("5. Failed Requests:");
            infoMessage.AppendLine("    a. Description:");
            infoMessage.AppendLine("        Failed Requests is the sum of requests that failed, calculated as Total Requests minus Successful Requests.");
            infoMessage.AppendLine("    b. Formula:");
            infoMessage.AppendLine("        Failed Requests = Total Requests − Successful Requests");
            infoMessage.AppendLine("6. Average Response Time:");
            infoMessage.AppendLine("    a. Description:");
            infoMessage.AppendLine("        Average Response Time represents the mean response time per request during the endurance testing.");
            infoMessage.AppendLine("    b. Formula:");
            infoMessage.AppendLine("        Average Response Time = Total Response Time / Total Requests");
            infoMessage.AppendLine("7. Throughput:");
            infoMessage.AppendLine("    a. Description:");
            infoMessage.AppendLine("        Throughput measures the number of successful requests processed per second during the endurance testing.");
            infoMessage.AppendLine("    b. Formula:");
            infoMessage.AppendLine("        Throughput = Successful Requests / Total Time Taken (in seconds)");
            infoMessage.AppendLine("8. Error Rate:");
            infoMessage.AppendLine("    a. Description:");
            infoMessage.AppendLine("        Error Rate indicates the percentage of requests that failed during the endurance testing.");
            infoMessage.AppendLine("    b. Formula:");
            infoMessage.AppendLine("        Error Rate = (Failed Requests / Total Requests) * 100");
            infoMessage.AppendLine("9. Round Duration:");
            infoMessage.AppendLine("    a. Description:");
            infoMessage.AppendLine("        Round Duration is the time taken to execute a round (in seconds), with a maximum value defined by Timeout.");
            infoMessage.AppendLine("    b. Formula:");
            infoMessage.AppendLine("        Round Duration = Actual time taken by a round (with a maximum value of Timeout)");

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
            textBoxTime.Clear();
            textBoxTimeout.Clear();
            lblTimeLeft.Text = "00:00:00:00";
            textBoxOutput.Clear();
            enduranceTestResults.Clear();
            totalRequests = 0;
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

                int row = 3;
                foreach (var result in enduranceTestResults)
                {
                    worksheet.Cell(row, 1).Value = result.Round;
                    worksheet.Cell(row, 2).Value = (int)result.StatusCode;
                    worksheet.Cell(row, 3).Value = result.ReasonPhrase;
                    worksheet.Cell(row, 4).Value = result.ResponseTime.TotalMilliseconds;
                    worksheet.Cell(row, 5).Value = result.CpuUsage;
                    worksheet.Cell(row, 6).Value = result.RamUsage;
                    worksheet.Cell(row, 7).Value = totalRequests;
                    worksheet.Cell(row, 8).Value = result.SuccessfulRequests;
                    worksheet.Cell(row, 9).Value = result.FailedRequests;
                    worksheet.Cell(row, 10).Value = result.AverageResponseTime;
                    worksheet.Cell(row, 11).Value = result.Throughput;
                    worksheet.Cell(row, 12).Value = result.ErrorRate;
                    row++;
                }

                int summaryStartRow = 1;
                int summaryStartColumn = 14;

                worksheet.Cell(summaryStartRow, summaryStartColumn).Value = "Overall Summary";
                worksheet.Cell(summaryStartRow, summaryStartColumn).Style.Font.Bold = true;
                worksheet.Cell(summaryStartRow, summaryStartColumn).Style.Font.FontSize = 14;
                worksheet.Cell(summaryStartRow, summaryStartColumn).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Range("N1:O1").Merge();

                int totalRequestsOverall = totalRequests * currentRound;
                int totalSuccessfulRequestsOverall = totalSuccessfulRequests;
                int totalFailedRequestsOverall = totalFailedRequests;
                float averageCpuUsageOverall = currentRound > 0 ? totalCpuUsage / currentRound : 0;
                float averageRamUsageOverall = currentRound > 0 ? totalRamUsage / currentRound : 0;
                float averageResponseTimeOverall = totalResponses > 0 ? totalResponseTime / totalResponses : 0;
                float averageThroughputOverall = totalResponses > 0 ? totalThroughput / currentRound : 0;
                float averageErrorRateOverall = totalResponses > 0 ? (totalFailedRequests / (float)totalRequestsOverall) * 100 : 0;

                summaryStartRow++;
                worksheet.Cell(summaryStartRow, summaryStartColumn).Value = "Total Requests:";
                worksheet.Cell(summaryStartRow, summaryStartColumn + 1).Value = totalRequestsOverall;

                summaryStartRow++;
                worksheet.Cell(summaryStartRow, summaryStartColumn).Value = "Successful Requests:";
                worksheet.Cell(summaryStartRow, summaryStartColumn + 1).Value = totalSuccessfulRequestsOverall;

                summaryStartRow++;
                worksheet.Cell(summaryStartRow, summaryStartColumn).Value = "Failed Requests:";
                worksheet.Cell(summaryStartRow, summaryStartColumn + 1).Value = totalFailedRequestsOverall;

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
                worksheet.Cell(summaryStartRow, summaryStartColumn + 1).Value = averageErrorRateOverall.ToString() + "%";

                int paramStartRow = 1;
                int paramStartColumn = 17;

                worksheet.Cell(paramStartRow, paramStartColumn).Value = "Test Parameter";
                worksheet.Cell(paramStartRow, paramStartColumn).Style.Font.Bold = true;
                worksheet.Cell(paramStartRow, paramStartColumn).Style.Font.FontSize = 14;
                worksheet.Cell(paramStartRow, paramStartColumn).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Range("Q1:R1").Merge();

                paramStartRow++;
                worksheet.Cell(paramStartRow, paramStartColumn).Value = "URL:";
                worksheet.Cell(paramStartRow, paramStartColumn + 1).Value = url;

                paramStartRow++;
                worksheet.Cell(paramStartRow, paramStartColumn).Value = "Number of Requests:";
                worksheet.Cell(paramStartRow, paramStartColumn + 1).Value = totalRequests;

                paramStartRow++;
                worksheet.Cell(paramStartRow, paramStartColumn).Value = "Timeout Per Round:";
                worksheet.Cell(paramStartRow, paramStartColumn + 1).Value = timeoutInSeconds + " second(s)";

                paramStartRow++;
                worksheet.Cell(paramStartRow, paramStartColumn).Value = "Time in Period:";
                worksheet.Cell(paramStartRow, paramStartColumn + 1).Value = durationInSeconds + " " + selectedTimePeriod;

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
        public float CpuUsage { get; set; }
        public float RamUsage { get; set; }
        public int SuccessfulRequests { get; set; }
        public int FailedRequests { get; set; }
        public float AverageResponseTime { get; set; }
        public float Throughput { get; set; }
        public float ErrorRate { get; set; }
    }
}
