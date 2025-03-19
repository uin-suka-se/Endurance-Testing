using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using Endurance_Testing.Core;
using Endurance_Testing.Helpers;
using Endurance_Testing.Models;
using Endurance_Testing.Services;
using Endurance_Testing.UI;

namespace Endurance_Testing
{
    public partial class EnduranceTesting : UI.MacStyleTitleBar
    {
        private TestRunner testRunner;
        private TestSummary testSummary;
        private CancellationTokenSource cancellationTokenSource;
        private List<EnduranceTestResult> enduranceTestResults = new List<EnduranceTestResult>();
        private List<string> roundResults = new List<string>();
        private bool isRunning = false;
        private string selectedTimePeriod;
        private string aiAnalysisResult = "";
        private ExcelExportService excelExportService;
        private CsvExportService csvExportService;
        private JsonExportService jsonExportService;
        private HtmlExportService htmlExportService;
        private DiscordWebhookService discordWebhookService;

        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]

        private static extern IntPtr CreateRoundRectRgn(
            int nLeftRect,
            int nTopRect,
            int nRightRect,
            int nBottomRect,
            int nWidthEllipse,
            int nHeightEllipse
        );

        public EnduranceTesting()
        {
            InitializeComponent();

            Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));

            this.Load += EnduranceTesting_Load;
            this.FormClosing += EnduranceTesting_FormClosing;

            testRunner = new TestRunner();
            testRunner.ResultReceived += TestRunner_ResultReceived;
            testRunner.RoundCompleted += TestRunner_RoundCompleted;
            testRunner.TestCompleted += TestRunner_TestCompleted;

            testSummary = new TestSummary();

            textBoxInputRequest.KeyPress += new KeyPressEventHandler(textBoxOnlyNumber_KeyPress);
            textBoxInputMaxRequest.KeyPress += new KeyPressEventHandler(textBoxOnlyNumber_KeyPress);
            textBoxTimeout.KeyPress += new KeyPressEventHandler(textBoxOnlyNumber_KeyPress);
            textBoxTime.KeyPress += new KeyPressEventHandler(textBoxOnlyNumber_KeyPress);
            radioButtonSecond.CheckedChanged += radioButton_CheckedChanged;
            radioButtonMinute.CheckedChanged += radioButton_CheckedChanged;
            radioButtonHour.CheckedChanged += radioButton_CheckedChanged;
            comboBoxMode.SelectedIndex = 0;
            comboBoxMode.SelectedIndexChanged += new EventHandler(comboBoxMode_SelectedIndexChanged);

            excelExportService = new ExcelExportService();
            csvExportService = new CsvExportService();
            jsonExportService = new JsonExportService();
            htmlExportService = new HtmlExportService();

            discordWebhookService = new DiscordWebhookService();
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
            else
            {
                Application.ExitThread();
            }
        }

        private void EnduranceTesting_Load(object sender, EventArgs e)
        {
            Color normalBorderColor = Color.FromArgb(42, 40, 60);
            Color focusedBorderColor = Color.FromArgb(85, 213, 219);
            Color activeForeColor = Color.FromArgb(28, 26, 43);

            textBoxInputUrl.MakeRounded(
                normalBorderColor: normalBorderColor,
                focusedBorderColor: focusedBorderColor,
                borderWidth: 2,
                shadowDepth: 5);

            textBoxInputRequest.MakeRounded(
                normalBorderColor: normalBorderColor,
                focusedBorderColor: focusedBorderColor,
                borderWidth: 2,
                shadowDepth: 5);

            textBoxInputMaxRequest.MakeRounded(
                normalBorderColor: normalBorderColor,
                focusedBorderColor: focusedBorderColor,
                borderWidth: 2,
                shadowDepth: 5);

            comboBoxMode.MakeRounded(
                normalBorderColor: normalBorderColor,
                focusedBorderColor: focusedBorderColor,
                borderWidth: 2,
                shadowDepth: 5);

            textBoxTimeout.MakeRounded(
                normalBorderColor: normalBorderColor,
                focusedBorderColor: focusedBorderColor,
                borderWidth: 2,
                shadowDepth: 5);

            textBoxTime.MakeRounded(
                normalBorderColor: normalBorderColor,
                focusedBorderColor: focusedBorderColor,
                borderWidth: 2,
                shadowDepth: 5);

            radioButtonSecond.MakeBold(
                foreColor: Color.Black,
                activeForeColor: activeForeColor,
                borderWidth: 2
            );

            radioButtonMinute.MakeBold(
                foreColor: Color.Black,
                activeForeColor: activeForeColor,
                borderWidth: 2
            );

            radioButtonHour.MakeBold(
                foreColor: Color.Black,
                activeForeColor: activeForeColor,
                borderWidth: 2
            );

            textBoxApiKey.MakeRounded(
                normalBorderColor: normalBorderColor,
                focusedBorderColor: focusedBorderColor,
                borderWidth: 2,
                shadowDepth: 5);

            textBoxDiscordWebhook.MakeRounded(
                normalBorderColor: normalBorderColor,
                focusedBorderColor: focusedBorderColor,
                borderWidth: 2,
                shadowDepth: 5);

            textBoxOutput.MakeRoundCorner(
                normalBorderColor: normalBorderColor,
                focusedBorderColor: focusedBorderColor,
                borderWidth: 2,
                cornerRadius: 10,
                shadowDepth: 5);

            btnStart.MakeRounded(
                Color.FromArgb(40, 167, 69),
                Color.White,
                Color.FromArgb(28, 117, 48),
                Color.FromArgb(230, 230, 230),
                10);

            btnStop.MakeRounded(
                Color.FromArgb(220, 53, 69),
                Color.White,
                Color.FromArgb(154, 37, 48),
                Color.FromArgb(230, 230, 230),
                10);

            btnHelp.MakeRounded(
                Color.FromArgb(23, 162, 184),
                Color.White,
                Color.FromArgb(16, 113, 129),
                Color.FromArgb(230, 230, 230),
                10);

            btnInfo.MakeRounded(
                Color.FromArgb(0, 123, 255),
                Color.White,
                Color.FromArgb(0, 86, 179),
                Color.FromArgb(230, 230, 230),
                10);

            btnClear.MakeRounded(
                Color.FromArgb(108, 117, 125),
                Color.White,
                Color.FromArgb(76, 82, 88),
                Color.FromArgb(230, 230, 230),
                10);

            btnExport.MakeRounded(
                Color.FromArgb(255, 153, 0),
                Color.White,
                Color.FromArgb(179, 107, 0),
                Color.FromArgb(230, 230, 230),
                10);

            textBoxInputUrl.Text = "https://example.com";
            btnStop.Enabled = false;
            btnExport.Enabled = false;
            selectedTimePeriod = "second(s)";
        }

        private void TestRunner_ResultReceived(object sender, ResultReceivedEventArgs e)
        {
            DisplayResult(e.Result, e.Round);
        }

        private void MarkRoundComplete()
        {
            testRunner.CompleteRound();
        }

        private void TestRunner_RoundCompleted(object sender, RoundCompletedEventArgs e)
        {
            textBoxOutput.Clear();

            foreach (var result in roundResults)
            {
                textBoxOutput.AppendText(result + Environment.NewLine);
            }
            roundResults.Clear();

            DisplayRoundStatistics(
                e.CpuUsage,
                e.RamUsage,
                e.SuccessfulRequests,
                e.FailedRequests,
                e.AverageLoadTime,
                e.AverageWaitTime,
                e.AverageResponseTime,
                e.Throughput,
                e.ErrorRate,
                e.RoundDuration,
                e.RequestCount
            );

            testSummary.CurrentRound = testRunner.CurrentRound;
            testSummary.TotalSuccessfulRequests = testRunner.TotalSuccessfulRequests;
            testSummary.TotalFailedRequests = testRunner.TotalFailedRequests;
            testSummary.TotalCpuUsage = testRunner.TotalCpuUsage;
            testSummary.TotalRamUsage = testRunner.TotalRamUsage;
            testSummary.TotalLoadTime = testRunner.TotalLoadTime;
            testSummary.TotalWaitTime = testRunner.TotalWaitTime;
            testSummary.TotalResponseTime = testRunner.TotalResponseTime;
            testSummary.TotalResponses = testRunner.TotalResponses;
            testSummary.TotalRequestsProcessed = testRunner.TotalRequestsProcessed;
            testSummary.TotalThroughput = testRunner.TotalThroughput;

            MarkRoundComplete();
        }

        private async void TestRunner_TestCompleted(object sender, EventArgs e)
        {
            enduranceTestResults = new List<EnduranceTestResult>(testRunner.GetResults());

            if (testRunner.CurrentRound > 0 && enduranceTestResults.Any())
            {
                await ShowSummary();
                await SendToDiscordAutomatically();
            }
            else
            {
                textBoxOutput.AppendText("Test completed with no results.");
                LogService.WriteLog("Test completed with no results.");
                textBoxOutput.ScrollToCaret();
            }

            btnStart.Enabled = true;
            btnStop.Enabled = false;
            btnClear.Enabled = true;
            btnExport.Enabled = true;
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

            if (!string.IsNullOrEmpty(textBoxInputRequest.Text) && int.TryParse(textBoxInputRequest.Text, out int value))
            {
                if (value > 1000)
                {
                    MessageBox.Show("Input request cannot exceed 1000.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBoxInputRequest.Text = "1000";
                    textBoxInputRequest.SelectionStart = textBoxInputRequest.Text.Length;
                }
            }
        }

        private void textBoxInputMaxRequest_TextChanged(object sender, EventArgs e)
        {
            if (textBoxInputMaxRequest.Text.Length > 0 && !isRunning)
            {
                btnClear.Enabled = true;
            }
            
            if (!string.IsNullOrEmpty(textBoxInputMaxRequest.Text) && int.TryParse(textBoxInputMaxRequest.Text, out int value))
            {
                if (value > 1000)
                {
                    MessageBox.Show("Input maximum request cannot exceed 1000.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBoxInputMaxRequest.Text = "1000";
                    textBoxInputMaxRequest.SelectionStart = textBoxInputMaxRequest.Text.Length;
                }
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

            if (textBoxTimeout.Text.Length > 6)
            {
                MessageBox.Show("Input timeout should be limited to 6 digits.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBoxTimeout.Text = textBoxTimeout.Text.Substring(0, 6);
                textBoxTimeout.SelectionStart = textBoxTimeout.Text.Length;
            }

        }

        private void textBoxTime_TextChanged(object sender, EventArgs e)
        {
            if (textBoxTime.Text.Length > 0 && !isRunning)
            {
                btnClear.Enabled = true;
            }

            if (textBoxTime.Text.Length > 6)
            {
                MessageBox.Show("Input time in period should be limited to 6 digits.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBoxTime.Text = textBoxTime.Text.Substring(0, 6);
                textBoxTime.SelectionStart = textBoxTime.Text.Length;
            }
        }

        private void radioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (!isRunning)
            {
                btnClear.Enabled = true;
            }
        }

        private void textBoxApiKey_TextChanged(object sender, EventArgs e)
        {
            if (textBoxApiKey.Text.Length > 0 && !isRunning)
            {
                btnClear.Enabled = true;
            }
        }

        private void textBoxApiKey_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.C)
            {
                e.SuppressKeyPress = true;
            }
        }

        private void textBoxApiKey_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.ControlKey)
            {
                e.Handled = true;
            }
        }

        private void textBoxApiKey_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (e is HandledMouseEventArgs handledEventArgs)
                {
                    handledEventArgs.Handled = true;
                }
            }
        }

        private void textBoxDiscordWebhook_TextChanged(object sender, EventArgs e)
        {
            if (textBoxDiscordWebhook.Text.Length > 0 && !isRunning)
            {
                btnClear.Enabled = true;
            }
        }

        private void textBoxDiscordWebhook_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.ControlKey)
            {
                e.Handled = true;
            }
        }

        private void textBoxDiscordWebhook_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (e is HandledMouseEventArgs handledEventArgs)
                {
                    handledEventArgs.Handled = true;
                }
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
                MessageBox.Show("Please enter a valid URL.",
                                "Invalid Input",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                isRunning = false;
                return;
            }
            testRunner.Url = url;

            if (!int.TryParse(textBoxInputRequest.Text, out int minRequests) || minRequests <= 0)
            {
                MessageBox.Show("Please enter a valid number of requests.",
                                "Invalid Input",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                isRunning = false;
                return;
            }
            testRunner.MinRequests = minRequests;

            string selectedMode = comboBoxMode.SelectedItem.ToString();
            testRunner.TestMode = selectedMode;

            if (selectedMode != "Stable")
            {
                if (!int.TryParse(textBoxInputMaxRequest.Text, out int maxRequests) || maxRequests <= 0)
                {
                    MessageBox.Show("Please enter a valid maximum number of requests.",
                                    "Invalid Input",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
                    isRunning = false;
                    return;
                }

                if (maxRequests <= minRequests)
                {
                    MessageBox.Show("Maximum requests must be greater than minimum requests.",
                                    "Invalid Input",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
                    isRunning = false;
                    return;
                }

                testRunner.MaxRequests = maxRequests;
                maxRequests = maxRequests;
            }
            else
            {
                testRunner.MaxRequests = minRequests;
            }

            if (!int.TryParse(textBoxTimeout.Text, out int timeoutValue) || timeoutValue <= 0)
            {
                MessageBox.Show("Please enter a valid timeout.",
                                "Invalid Input",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                isRunning = false;
                return;
            }
            testRunner.TimeoutInSeconds = timeoutValue;

            if (!long.TryParse(textBoxTime.Text, out long durationValue) || durationValue <= 0)
            {
                MessageBox.Show("Please enter a valid duration.",
                                "Invalid Input",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                isRunning = false;
                return;
            }

            if (radioButtonMinute.Checked)
            {
                durationValue *= 60;
                selectedTimePeriod = "minute(s)";
            }
            else if (radioButtonHour.Checked)
            {
                durationValue *= 3600;
                selectedTimePeriod = "hour(s)";
            }
            else if (radioButtonSecond.Checked)
            {
                selectedTimePeriod = "second(s)";
            }
            else
            {
                MessageBox.Show("Please select a time period (seconds, minutes, or hours).",
                                "Invalid Input",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                isRunning = false;
                return;
            }

            long maxDurationInSeconds = 86400 * 3;
            if (durationValue > maxDurationInSeconds)
            {
                MessageBox.Show("Duration exceeds the maximum limit of 3 days (72 hours, 4320 minutes, or 259200 seconds).",
                                "Duration Too Long",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                isRunning = false;
                return;
            }

            string logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "EnduranceTestLog.txt");
            if (File.Exists(logFilePath))
            {
                var deleteLogResult = MessageBox.Show("Log file already exists. Do you want to delete the existing log file to proceed with the test?",
                                                       "Confirm Deletion",
                                                       MessageBoxButtons.YesNo,
                                                       MessageBoxIcon.Warning);

                if (deleteLogResult == DialogResult.No)
                {
                    return;
                }
                else
                {
                    File.Delete(logFilePath);
                }
            }

            LogService.InitializeLog();

            testRunner.DurationInSeconds = durationValue;

            textBoxOutput.Clear();
            lblTimeLeft.Text = "00:00:00:00";
            enduranceTestResults.Clear();

            btnStart.Enabled = false;
            btnStop.Enabled = true;
            btnClear.Enabled = false;
            btnExport.Enabled = false;

            testSummary.CurrentRound = 0;
            testSummary.TotalSuccessfulRequests = 0;
            testSummary.TotalFailedRequests = 0;
            testSummary.TotalCpuUsage = 0;
            testSummary.TotalRamUsage = 0;
            testSummary.TotalLoadTime = 0;
            testSummary.TotalWaitTime = 0;
            testSummary.TotalResponseTime = 0;
            testSummary.TotalResponses = 0;
            testSummary.TotalRequestsProcessed = 0;
            testSummary.TotalThroughput = 0;
            aiAnalysisResult = "";

            cancellationTokenSource = new CancellationTokenSource();

            var countdownTask = StartCountdown(durationValue, cancellationTokenSource.Token);

            try
            {
                await testRunner.RunTest(cancellationTokenSource.Token);

                try
                {
                    await countdownTask;
                }
                catch (TaskCanceledException)
                {
                    // Handle exception
                }
            }
            catch (TaskCanceledException)
            {
                // Handle exception
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

                enduranceTestResults = testRunner.GetResults();
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

        private void DisplayResult(EnduranceTestResult result, int round)
        {
            string resultString = $"Round {round}: Status: {(int)result.StatusCode}, Reason: {result.ReasonPhrase}, Load Time: {result.LoadTime.TotalMilliseconds} ms, Wait Time: {result.WaitTime.TotalMilliseconds} ms, Response Time: {result.ResponseTime.TotalMilliseconds} ms";
            textBoxOutput.AppendText(resultString + Environment.NewLine);
            roundResults.Add(resultString);
            LogService.WriteLog(resultString + Environment.NewLine);
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
            StringBuilder roundStats = new StringBuilder();
            roundStats.AppendLine();
            roundStats.AppendLine($"Round {testRunner.CurrentRound} Statistics:");
            roundStats.AppendLine($"Computer's CPU Usage: {currentCpuUsage}%");
            roundStats.AppendLine($"Computer's RAM Usage: {currentRamUsage} MB");
            roundStats.AppendLine($"Total Requests: {currentRequests}");
            roundStats.AppendLine($"Successful Requests: {roundSuccessfulRequests}");
            roundStats.AppendLine($"Failed Requests: {roundFailedRequests}");
            roundStats.AppendLine($"Average Load Time: {averageRoundLoadTime} ms");
            roundStats.AppendLine($"Average Wait Time: {averageRoundWaitTime} ms");
            roundStats.AppendLine($"Average Response Time: {averageRoundResponseTime} ms");
            roundStats.AppendLine($"Throughput: {throughput} requests/second");
            roundStats.AppendLine($"Error Rate: {errorRate}%");
            roundStats.AppendLine($"Round Duration: {roundDuration} seconds");
            roundStats.AppendLine();

            string roundStatsString = roundStats.ToString();

            textBoxOutput.AppendText(roundStatsString);
            LogService.WriteLog(roundStatsString);
            textBoxOutput.ScrollToCaret();
        }

        private async Task ShowSummary()
        {
            double averageCpuUsage = testRunner.CurrentRound > 0 ? testRunner.TotalCpuUsage / testRunner.CurrentRound : 0;
            double averageRamUsage = testRunner.CurrentRound > 0 ? testRunner.TotalRamUsage / testRunner.CurrentRound : 0;

            double averageLoadTime = testRunner.TotalResponses > 0 ? testRunner.TotalLoadTime / testRunner.TotalResponses : 0;
            double averageWaitTime = testRunner.TotalResponses > 0 ? testRunner.TotalWaitTime / testRunner.TotalResponses : 0;
            double averageResponseTime = testRunner.TotalResponses > 0 ? testRunner.TotalResponseTime / testRunner.TotalResponses : 0;
            double averageThroughput = testRunner.TotalResponses > 0 ? testRunner.TotalThroughput / testRunner.CurrentRound : 0;

            double averageRoundDuration;
            var results = testRunner.GetResults();
            if (testRunner.CurrentRound > 0 && results.Any())
            {
                averageRoundDuration = results.Average(result => result.RoundDuration);
            }
            else
            {
                averageRoundDuration = 0;
            }

            string summaryMessage = $"Summary:{Environment.NewLine}" +
                                    $"Total Requests: {testRunner.TotalRequestsProcessed}{Environment.NewLine}" +
                                    $"Successful Requests: {testRunner.TotalSuccessfulRequests}{Environment.NewLine}" +
                                    $"Failed Requests: {testRunner.TotalFailedRequests}{Environment.NewLine}" +
                                    $"Average Computer's CPU Usage: {averageCpuUsage}%{Environment.NewLine}" +
                                    $"Average Computer's RAM Usage: {averageRamUsage} MB{Environment.NewLine}" +
                                    $"Average Load Time: {averageLoadTime} ms{Environment.NewLine}" +
                                    $"Average Wait Time: {averageWaitTime} ms{Environment.NewLine}" +
                                    $"Average Response Time: {averageResponseTime} ms{Environment.NewLine}" +
                                    $"Average Throughput: {averageThroughput} requests/second{Environment.NewLine}";

            if (testRunner.TotalRequestsProcessed > 0)
            {
                summaryMessage += $"Average Error Rate: {(testRunner.TotalFailedRequests / (double)testRunner.TotalRequestsProcessed) * 100}%{Environment.NewLine}";
            }
            else
            {
                summaryMessage += $"Average Error Rate: 0%{Environment.NewLine}";
            }

            summaryMessage += $"Average Round Duration: {averageRoundDuration} seconds";

            textBoxOutput.AppendText(summaryMessage);
            LogService.WriteLog(summaryMessage);
            textBoxOutput.ScrollToCaret();

            if (!string.IsNullOrWhiteSpace(textBoxApiKey.Text))
            {
                textBoxOutput.AppendText(Environment.NewLine + Environment.NewLine + "Fetching AI analysis..." + Environment.NewLine);
                LogService.WriteLog(Environment.NewLine + Environment.NewLine + "Fetching AI analysis..." + Environment.NewLine);
                textBoxOutput.ScrollToCaret();

                string url = testRunner.Url;
                double averageErrorRate = (testRunner.TotalFailedRequests / (double)testRunner.TotalRequestsProcessed) * 100;

                aiAnalysisResult = await AIAnalysisService.GetAIAnalysis(
                    textBoxApiKey.Text.Trim(),
                    url,
                    averageLoadTime,
                    averageWaitTime,
                    averageResponseTime,
                    averageThroughput,
                    averageErrorRate,
                    testRunner.TotalSuccessfulRequests,
                    testRunner.TotalFailedRequests,
                    testRunner.TotalRequestsProcessed);

                if (!string.IsNullOrEmpty(aiAnalysisResult))
                {
                    string aiResultHeader = Environment.NewLine + Environment.NewLine +
                                           "============ AI ANALYSIS ============";
                    string aiResultFooter = Environment.NewLine +
                                           "=====================================";

                    textBoxOutput.AppendText(aiResultHeader + Environment.NewLine + Environment.NewLine);
                    textBoxOutput.AppendText(aiAnalysisResult + Environment.NewLine);
                    textBoxOutput.AppendText(aiResultFooter);
                    LogService.WriteLog(aiResultHeader + Environment.NewLine + Environment.NewLine);
                    LogService.WriteLog(aiAnalysisResult + Environment.NewLine);
                    LogService.WriteLog(aiResultFooter);
                    textBoxOutput.ScrollToCaret();
                }
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
            string helpMessage = HelpManager.GenerateHelpMessage();
            MessageBox.Show(helpMessage, "User Guide", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnInfo_Click(object sender, EventArgs e)
        {
            ShowInfoMessageBox();
        }

        private void ShowInfoMessageBox()
        {
            string infoMessage = Helpers.InfoManager.GenerateInfoMessage();
            MessageBox.Show(infoMessage, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            radioButtonSecond.Checked = false;
            radioButtonMinute.Checked = false;
            radioButtonHour.Checked = false;
            lblTimeLeft.Text = "00:00:00:00";
            textBoxOutput.Clear();
            enduranceTestResults.Clear();
            textBoxApiKey.Clear();
            textBoxDiscordWebhook.Clear();
            testSummary.CurrentRound = 0;
            testSummary.TotalSuccessfulRequests = 0;
            testSummary.TotalFailedRequests = 0;
            testSummary.TotalCpuUsage = 0;
            testSummary.TotalRamUsage = 0;
            testSummary.TotalLoadTime = 0;
            testSummary.TotalWaitTime = 0;
            testSummary.TotalResponseTime = 0;
            testSummary.TotalResponses = 0;
            testSummary.TotalRequestsProcessed = 0;
            testSummary.TotalThroughput = 0;
            aiAnalysisResult = "";
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
            exportMenu.Items.Add("Excel (.xlsx)", null, (s, args) => ExportToExcel());
            exportMenu.Items.Add("CSV (.csv)", null, (s, args) => ExportToCsv());
            exportMenu.Items.Add("JSON (.json)", null, (s, args) => ExportToJson());
            exportMenu.Items.Add("HTML (.html)", null, (s, args) => ExportToHtml());
            exportMenu.Items.Add("Discord (Test Summary)", null, (s, args) => SendToDiscord());

            exportMenu.Show(btnExport, new Point(0, btnExport.Height));
        }

        private void ExportToExcel()
        {
            TestSummary summary = new TestSummary
            {
                TotalRequestsProcessed = testSummary.TotalRequestsProcessed,
                TotalSuccessfulRequests = testSummary.TotalSuccessfulRequests,
                TotalFailedRequests = testSummary.TotalFailedRequests,
                CurrentRound = testSummary.CurrentRound,
                TotalCpuUsage = testSummary.TotalCpuUsage,
                TotalRamUsage = testSummary.TotalRamUsage,
                TotalLoadTime = testSummary.TotalLoadTime,
                TotalWaitTime = testSummary.TotalWaitTime,
                TotalResponseTime = testSummary.TotalResponseTime,
                TotalResponses = testSummary.TotalResponses,
                TotalThroughput = testSummary.TotalThroughput
            };

            if (testRunner.CurrentRound > 0 && enduranceTestResults.Any())
            {
                summary.AverageRoundDuration = enduranceTestResults.Average(result => result.RoundDuration);
            }

            TestParameters parameters = new TestParameters
            {
                Url = testRunner.Url,
                MinRequests = testRunner.MinRequests,
                MaxRequests = testRunner.MaxRequests,
                Mode = testRunner.TestMode,
                TimeoutInSeconds = testRunner.TimeoutInSeconds,
                DurationInSeconds = testRunner.DurationInSeconds,
                SelectedTimePeriod = selectedTimePeriod
            };

            excelExportService.ExportToExcel(enduranceTestResults, summary, parameters, aiAnalysisResult);
        }

        private void ExportToCsv()
        {
            TestSummary summary = new TestSummary
            {
                TotalRequestsProcessed = testSummary.TotalRequestsProcessed,
                TotalSuccessfulRequests = testSummary.TotalSuccessfulRequests,
                TotalFailedRequests = testSummary.TotalFailedRequests,
                CurrentRound = testSummary.CurrentRound,
                TotalCpuUsage = testSummary.TotalCpuUsage,
                TotalRamUsage = testSummary.TotalRamUsage,
                TotalLoadTime = testSummary.TotalLoadTime,
                TotalWaitTime = testSummary.TotalWaitTime,
                TotalResponseTime = testSummary.TotalResponseTime,
                TotalResponses = testSummary.TotalResponses,
                TotalThroughput = testSummary.TotalThroughput
            };

            if (testRunner.CurrentRound > 0 && enduranceTestResults.Any())
            {
                summary.AverageRoundDuration = enduranceTestResults.Average(result => result.RoundDuration);
            }

            TestParameters parameters = new TestParameters
            {
                Url = testRunner.Url,
                MinRequests = testRunner.MinRequests,
                MaxRequests = testRunner.MaxRequests,
                Mode = testRunner.TestMode,
                TimeoutInSeconds = testRunner.TimeoutInSeconds,
                DurationInSeconds = testRunner.DurationInSeconds,
                SelectedTimePeriod = selectedTimePeriod
            };

            csvExportService.ExportToCsv(enduranceTestResults, summary, parameters, aiAnalysisResult);
        }

        private void ExportToJson()
        {
            TestSummary summary = new TestSummary
            {
                TotalRequestsProcessed = testSummary.TotalRequestsProcessed,
                TotalSuccessfulRequests = testSummary.TotalSuccessfulRequests,
                TotalFailedRequests = testSummary.TotalFailedRequests,
                CurrentRound = testSummary.CurrentRound,
                TotalCpuUsage = testSummary.TotalCpuUsage,
                TotalRamUsage = testSummary.TotalRamUsage,
                TotalLoadTime = testSummary.TotalLoadTime,
                TotalWaitTime = testSummary.TotalWaitTime,
                TotalResponseTime = testSummary.TotalResponseTime,
                TotalResponses = testSummary.TotalResponses,
                TotalThroughput = testSummary.TotalThroughput
            };

            if (testRunner.CurrentRound > 0 && enduranceTestResults.Any())
            {
                summary.AverageRoundDuration = enduranceTestResults.Average(result => result.RoundDuration);
            }

            TestParameters parameters = new TestParameters
            {
                Url = testRunner.Url,
                MinRequests = testRunner.MinRequests,
                MaxRequests = testRunner.MaxRequests,
                Mode = testRunner.TestMode,
                TimeoutInSeconds = testRunner.TimeoutInSeconds,
                DurationInSeconds = testRunner.DurationInSeconds,
                SelectedTimePeriod = selectedTimePeriod
            };

            jsonExportService.ExportToJson(enduranceTestResults, summary, parameters, aiAnalysisResult);
        }

        private void ExportToHtml()
        {
            if (enduranceTestResults != null && enduranceTestResults.Count > 0)
            {
                TestSummary summary = new TestSummary
                {
                    TotalRequestsProcessed = testSummary.TotalRequestsProcessed,
                    TotalSuccessfulRequests = testSummary.TotalSuccessfulRequests,
                    TotalFailedRequests = testSummary.TotalFailedRequests,
                    CurrentRound = testSummary.CurrentRound,
                    TotalCpuUsage = testSummary.TotalCpuUsage,
                    TotalRamUsage = testSummary.TotalRamUsage,
                    TotalLoadTime = testSummary.TotalLoadTime,
                    TotalWaitTime = testSummary.TotalWaitTime,
                    TotalResponseTime = testSummary.TotalResponseTime,
                    TotalResponses = testSummary.TotalResponses,
                    TotalThroughput = testSummary.TotalThroughput
                };

                if (testRunner.CurrentRound > 0 && enduranceTestResults.Any())
                {
                    summary.AverageRoundDuration = enduranceTestResults.Average(result => result.RoundDuration);
                }

                TestParameters parameters = new TestParameters
                {
                    Url = testRunner.Url,
                    MinRequests = testRunner.MinRequests,
                    MaxRequests = testRunner.MaxRequests,
                    Mode = testRunner.TestMode,
                    TimeoutInSeconds = testRunner.TimeoutInSeconds,
                    DurationInSeconds = testRunner.DurationInSeconds,
                    SelectedTimePeriod = selectedTimePeriod
                };

                htmlExportService.ExportToHtml(
                    enduranceTestResults,
                    summary,
                    parameters,
                    aiAnalysisResult);
            }
            else
            {
                MessageBox.Show("There is no test result to export.", "Information",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private async Task SendToDiscordAutomatically()
        {
            string webhookUrl = textBoxDiscordWebhook.Text.Trim();

            if (!string.IsNullOrEmpty(webhookUrl))
            {
                try
                {
                    TestSummary summary = new TestSummary
                    {
                        TotalRequestsProcessed = testSummary.TotalRequestsProcessed,
                        TotalSuccessfulRequests = testSummary.TotalSuccessfulRequests,
                        TotalFailedRequests = testSummary.TotalFailedRequests,
                        CurrentRound = testSummary.CurrentRound,
                        TotalCpuUsage = testSummary.TotalCpuUsage,
                        TotalRamUsage = testSummary.TotalRamUsage,
                        TotalLoadTime = testSummary.TotalLoadTime,
                        TotalWaitTime = testSummary.TotalWaitTime,
                        TotalResponseTime = testSummary.TotalResponseTime,
                        TotalResponses = testSummary.TotalResponses,
                        TotalThroughput = testSummary.TotalThroughput
                    };

                    if (testRunner.CurrentRound > 0 && enduranceTestResults.Any())
                    {
                        summary.AverageRoundDuration = enduranceTestResults.Average(result => result.RoundDuration);
                    }

                    TestParameters parameters = new TestParameters
                    {
                        Url = testRunner.Url,
                        MinRequests = testRunner.MinRequests,
                        MaxRequests = testRunner.MaxRequests,
                        Mode = testRunner.TestMode,
                        TimeoutInSeconds = testRunner.TimeoutInSeconds,
                        DurationInSeconds = testRunner.DurationInSeconds,
                        SelectedTimePeriod = selectedTimePeriod
                    };

                    bool success = await discordWebhookService.SendToDiscord(webhookUrl, summary, parameters, aiAnalysisResult);

                    if (success)
                    {
                        textBoxOutput.AppendText("\r\n[INFO] Test summary successfully sent to Discord!");
                        LogService.WriteLog("\r\n[INFO] Test summary successfully sent to Discord!");
                        textBoxOutput.ScrollToCaret();
                    }
                    else
                    {
                        textBoxOutput.AppendText("\r\n[ERROR] Failed to send test summary to Discord. Check your webhook URL and internet connection.");
                        LogService.WriteLog("\r\n[ERROR] Failed to send test summary to Discord. Check your webhook URL and internet connection.");
                        textBoxOutput.ScrollToCaret();
                    }
                }
                catch (Exception ex)
                {
                    textBoxOutput.AppendText($"\r\n[ERROR] Error sending to Discord: {ex.Message}");
                    LogService.WriteLog($"\r\n[ERROR] Error sending to Discord: {ex.Message}");
                    textBoxOutput.ScrollToCaret();
                }
            }
        }

        private async void SendToDiscord()
        {
            string webhookUrl = textBoxDiscordWebhook.Text.Trim();

            if (string.IsNullOrEmpty(webhookUrl))
            {
                MessageBox.Show("Please enter a Discord webhook URL",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (enduranceTestResults == null || enduranceTestResults.Count == 0)
            {
                MessageBox.Show("No test summary available to send",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                TestSummary summary = new TestSummary
                {
                    TotalRequestsProcessed = testSummary.TotalRequestsProcessed,
                    TotalSuccessfulRequests = testSummary.TotalSuccessfulRequests,
                    TotalFailedRequests = testSummary.TotalFailedRequests,
                    CurrentRound = testSummary.CurrentRound,
                    TotalCpuUsage = testSummary.TotalCpuUsage,
                    TotalRamUsage = testSummary.TotalRamUsage,
                    TotalLoadTime = testSummary.TotalLoadTime,
                    TotalWaitTime = testSummary.TotalWaitTime,
                    TotalResponseTime = testSummary.TotalResponseTime,
                    TotalResponses = testSummary.TotalResponses,
                    TotalThroughput = testSummary.TotalThroughput
                };

                if (testRunner.CurrentRound > 0 && enduranceTestResults.Any())
                {
                    summary.AverageRoundDuration = enduranceTestResults.Average(result => result.RoundDuration);
                }

                TestParameters parameters = new TestParameters
                {
                    Url = testRunner.Url,
                    MinRequests = testRunner.MinRequests,
                    MaxRequests = testRunner.MaxRequests,
                    Mode = testRunner.TestMode,
                    TimeoutInSeconds = testRunner.TimeoutInSeconds,
                    DurationInSeconds = testRunner.DurationInSeconds,
                    SelectedTimePeriod = selectedTimePeriod
                };

                bool success = await discordWebhookService.SendToDiscord(webhookUrl, summary, parameters, aiAnalysisResult);

                if (success)
                {
                    MessageBox.Show("Test summary successfully sent to Discord!",
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Failed to send test summary to Discord. Check your webhook URL and internet connection.",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error sending to Discord: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}