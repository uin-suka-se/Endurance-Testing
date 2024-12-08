using System;
using System.Collections.Generic;
using ClosedXML.Excel;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Endurance_Testing
{
    public partial class EnduranceTesting : Form
    {
        private CancellationTokenSource cancellationTokenSource;
        private List<EnduranceTestResult> enduranceTestResults = new List<EnduranceTestResult>();
        private int totalRequests;
        private int durationInSeconds;
        private int currentRound;
        private int totalSuccessfulRequests;
        private int totalFailedRequests;
        private float totalCpuUsage;
        private float totalRamUsage;
        private float totalResponseTime;
        private int totalResponses;

        public EnduranceTesting()
        {
            InitializeComponent();
            this.Load += EnduranceTesting_Load;
            textBoxInputRequest.KeyPress += new KeyPressEventHandler(textBoxOnlyNumber_KeyPress);
            textBoxTime.KeyPress += new KeyPressEventHandler(textBoxOnlyNumber_KeyPress);

            btnStop.Enabled = false;
        }

        private void EnduranceTesting_Load(object sender, EventArgs e)
        {
            // Inisialisasi jika diperlukan
        }

        private void textBoxOnlyNumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private async void btnStart_Click(object sender, EventArgs e)
        {
            string url = textBoxInputUrl.Text;
            if (string.IsNullOrWhiteSpace(url))
            {
                MessageBox.Show("Please enter a valid URL.");
                return;
            }

            if (!int.TryParse(textBoxInputRequest.Text, out totalRequests) || totalRequests <= 0)
            {
                MessageBox.Show("Please enter a valid number of requests.");
                return;
            }

            if (!int.TryParse(textBoxTime.Text, out durationInSeconds) || durationInSeconds <= 0)
            {
                MessageBox.Show("Please enter a valid duration.");
                return;
            }

            if (radioButtonMinute.Checked)
            {
                durationInSeconds *= 60;
            }
            else if (radioButtonHour.Checked)
            {
                durationInSeconds *= 3600;
            }
            else if (!radioButtonSecond.Checked)
            {
                MessageBox.Show("Please select a time period (seconds, minutes, or hours).");
                return;
            }

            btnStart.Enabled = false;
            btnStop.Enabled = true;
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

            var countdownTask = StartCountdown(durationInSeconds, cancellationTokenSource.Token);

            await RunEnduranceTest(url, cancellationTokenSource.Token);

            await countdownTask;

            btnClear.Enabled = true;
            btnExport.Enabled = true;

            ShowSummary();
        }

        private async Task StartCountdown(int durationInSeconds, CancellationToken cancellationToken)
        {
            for (int i = durationInSeconds; i > 0; i--)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                TimeSpan timeLeft = TimeSpan.FromSeconds(i);
                lblTimeLeft.Text = $"{timeLeft:hh\\:mm\\:ss}";
                await Task.Delay(1000);
            }

            lblTimeLeft.Text = "00:00:00";

            cancellationTokenSource.Cancel();
        }

        private async Task RunEnduranceTest(string url, CancellationToken cancellationToken)
        {
            HttpClient httpClient = new HttpClient();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            while (!cancellationToken.IsCancellationRequested)
            {
                currentRound++;

                int roundSuccessfulRequests = 0;
                int roundFailedRequests = 0;
                float roundResponseTime = 0;

                var tasks = new List<Task<EnduranceTestResult>>();
                for (int i = 0; i < totalRequests; i++)
                {
                    tasks.Add(SendHttpRequest(httpClient, url, currentRound));
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
                    }
                }

                if (totalRequests > 0)
                {
                    float averageRoundResponseTime = roundResponseTime / totalRequests;

                    foreach (var result in results)
                    {
                        result.CpuUsage = roundCpuUsage;
                        result.RamUsage = roundRamUsage;
                        result.SuccessfulRequests = roundSuccessfulRequests;
                        result.FailedRequests = roundFailedRequests;
                        result.AverageResponseTime = averageRoundResponseTime;
                    }

                    totalCpuUsage += roundCpuUsage;
                    totalRamUsage += roundRamUsage;
                    totalResponseTime += roundResponseTime;
                    totalResponses += totalRequests;

                    DisplayRoundStatistics(roundCpuUsage, roundRamUsage, roundSuccessfulRequests, roundFailedRequests, averageRoundResponseTime);
                }

                await Task.Delay(1000);
            }

            stopwatch.Stop();
            btnStart.Enabled = true;
            btnStop.Enabled = false;
        }

        private async Task<EnduranceTestResult> SendHttpRequest(HttpClient httpClient, string url, int round)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(url);
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
            string resultString = $"Round {round}: Status: {result.StatusCode}, Reason: {result.ReasonPhrase}, Response Time: {result.ResponseTime.TotalMilliseconds} ms";
            textBoxOutput.AppendText(resultString + Environment.NewLine);
            textBoxOutput.ScrollToCaret();
        }

        private void DisplayRoundStatistics(float currentCpuUsage, float currentRamUsage, int roundSuccessfulRequests, int roundFailedRequests, float averageRoundResponseTime)
        {
            string roundStats = $"Round {currentRound} Statistics:{Environment.NewLine}" +
                                $"CPU Usage: {currentCpuUsage:F2}%{Environment.NewLine}" +
                                $"RAM Usage: {currentRamUsage:F2} MB{Environment.NewLine}" +
                                $"Total Requests: {totalRequests}{Environment.NewLine}" +
                                $"Successful Requests: {roundSuccessfulRequests}{Environment.NewLine}" +
                                $"Failed Requests: {roundFailedRequests}{Environment.NewLine}" +
                                $"Average Response Time: {averageRoundResponseTime:F2} ms{Environment.NewLine}{Environment.NewLine}";

            textBoxOutput.AppendText(roundStats);
            textBoxOutput.ScrollToCaret();
        }

        private void ShowSummary()
        {
            float averageCpuUsage = currentRound > 0 ? totalCpuUsage / currentRound : 0;
            float averageRamUsage = currentRound > 0 ? totalRamUsage / currentRound : 0;

            float averageResponseTime = totalResponses > 0 ? totalResponseTime / totalResponses : 0;

            string summaryMessage = $"Summary:{Environment.NewLine}" +
                                    $"Total Requests: {totalRequests * currentRound}{Environment.NewLine}" +
                                    $"Successful Requests: {totalSuccessfulRequests}{Environment.NewLine}" +
                                    $"Failed Requests: {totalFailedRequests}{Environment.NewLine}" +
                                    $"Average CPU Usage: {averageCpuUsage:F2}%{Environment.NewLine}" +
                                    $"Average RAM Usage: {averageRamUsage:F2} MB{Environment.NewLine}" +
                                    $"Average Response Time: {averageResponseTime:F2} ms{Environment.NewLine}";

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
            cancellationTokenSource.Cancel();
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
            return "User Guide for Endurance Testing:\n" +
                   "1. Enter the target URL in the URL column.\n" +
                   "2. Enter the number of requests in the Requests column.\n" +
                   "3. Enter the duration for the endurance test in the Time column.\n" +
                   "4. Select the time period (seconds, minutes, hours).\n" +
                   "5. Click the 'Start' button to begin the endurance testing.\n" +
                   "6. Monitor the results in the output area.\n" +
                   "7. Click 'Stop' to halt the testing process.\n" +
                   "8. Use 'Clear' to reset the input fields and output.";
        }

        private void btnInfo_Click(object sender, EventArgs e)
        {
            ShowInfoMessageBox();
        }

        private void ShowInfoMessageBox()
        {
            string infoMessage = "Endurance Testing:\n" +
                                 "Endurance testing is a type of performance testing that determines how a system behaves under sustained load. It helps identify performance issues and ensures the system can handle prolonged usage.";
            MessageBox.Show(infoMessage, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            textBoxInputUrl.Clear();
            textBoxInputRequest.Clear();
            textBoxTime.Clear();
            lblTimeLeft.Text = "00:00:00";
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
                worksheet.Range("A1:J1").Merge();

                worksheet.Cell(2, 1).Value = "Round";
                worksheet.Cell(2, 2).Value = "Status";
                worksheet.Cell(2, 3).Value = "Reason";
                worksheet.Cell(2, 4).Value = "Response Time (ms)";
                worksheet.Cell(2, 5).Value = "CPU Usage (%)";
                worksheet.Cell(2, 6).Value = "RAM Usage (MB)";
                worksheet.Cell(2, 7).Value = "Total Requests";
                worksheet.Cell(2, 8).Value = "Successful Requests";
                worksheet.Cell(2, 9).Value = "Failed Requests";
                worksheet.Cell(2, 10).Value = "Average Response Time (ms)";

                int row = 3;
                foreach (var result in enduranceTestResults)
                {
                    worksheet.Cell(row, 1).Value = result.Round;
                    worksheet.Cell(row, 2).Value = result.StatusCode.ToString();
                    worksheet.Cell(row, 3).Value = result.ReasonPhrase;
                    worksheet.Cell(row, 4).Value = result.ResponseTime.TotalMilliseconds;
                    worksheet.Cell(row, 5).Value = result.CpuUsage;
                    worksheet.Cell(row, 6).Value = result.RamUsage;
                    worksheet.Cell(row, 7).Value = totalRequests;
                    worksheet.Cell(row, 8).Value = result.SuccessfulRequests;
                    worksheet.Cell(row, 9).Value = result.FailedRequests;
                    worksheet.Cell(row, 10).Value = result.AverageResponseTime;
                    row++;
                }

                row += 2;
                worksheet.Cell(row, 1).Value = "Overall Summary";
                worksheet.Cell(row, 1).Style.Font.Bold = true;
                worksheet.Cell(row, 1).Style.Font.FontSize = 14;

                int totalRequestsOverall = totalRequests * currentRound;
                int totalSuccessfulRequestsOverall = totalSuccessfulRequests;
                int totalFailedRequestsOverall = totalFailedRequests;
                float averageCpuUsageOverall = currentRound > 0 ? totalCpuUsage / currentRound : 0;
                float averageRamUsageOverall = currentRound > 0 ? totalRamUsage / currentRound : 0;
                float averageResponseTimeOverall = totalResponses > 0 ? totalResponseTime / totalResponses : 0;

                row++;
                worksheet.Cell(row, 1).Value = "Total Requests:";
                worksheet.Cell(row, 2).Value = totalRequestsOverall;

                row++;
                worksheet.Cell(row, 1).Value = "Successful Requests:";
                worksheet.Cell(row, 2).Value = totalSuccessfulRequestsOverall;

                row++;
                worksheet.Cell(row, 1).Value = "Failed Requests:";
                worksheet.Cell(row, 2).Value = totalFailedRequestsOverall;

                row++;
                worksheet.Cell(row, 1).Value = "Average CPU Usage:";
                worksheet.Cell(row, 2).Value = averageCpuUsageOverall.ToString("F2") + "%";

                row++;
                worksheet.Cell(row, 1).Value = "Average RAM Usage:";
                worksheet.Cell(row, 2).Value = averageRamUsageOverall.ToString("F2") + " MB";

                row++;
                worksheet.Cell(row, 1).Value = "Average Response Time:";
                worksheet.Cell(row, 2).Value = averageResponseTimeOverall.ToString("F2") + " ms";

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
    }
}
