using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Endurance_Testing.Models;

namespace Endurance_Testing.Services
{
    public class HtmlExportService
    {
        public bool ExportToHtml(List<EnduranceTestResult> testResults, TestSummary testSummary,
                               TestParameters testParameters, string aiAnalysisResult = "")
        {
            try
            {
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = "HTML Files|*.html";
                    saveFileDialog.Title = "Save an HTML File";
                    saveFileDialog.FileName = "EnduranceTestResults.html";

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string htmlContent = GenerateHtmlContent(testResults, testSummary, testParameters, aiAnalysisResult);
                        File.WriteAllText(saveFileDialog.FileName, htmlContent);

                        MessageBox.Show("HTML export successful!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to export to HTML: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return false;
        }

        public async Task<bool> ExportToHtmlAsync(
            List<EnduranceTestResult> testResults,
            TestSummary testSummary,
            TestParameters testParameters,
            string aiAnalysisResult = "")
        {
            return await Task.Run(() => ExportToHtml(testResults, testSummary, testParameters, aiAnalysisResult));
        }

        private string GenerateHtmlContent(
            List<EnduranceTestResult> testResults,
            TestSummary testSummary,
            TestParameters testParameters,
            string aiAnalysisResult)
        {
            StringBuilder html = new StringBuilder();

            html.AppendLine("<!DOCTYPE html>");
            html.AppendLine("<html lang=\"en\">");
            html.AppendLine("<head>");
            html.AppendLine("    <meta charset=\"UTF-8\">");
            html.AppendLine("    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
            html.AppendLine("    <title>Endurance Test Results</title>");
            html.AppendLine("    <style>");
            html.AppendLine("        body { font-family: Arial, sans-serif; margin: 0; padding: 0; color: #333; line-height: 1.6; }");
            html.AppendLine("        h1, h2, h3 { color: #444; }");
            html.AppendLine("        .container { max-width: 1200px; margin: 0 auto; padding: 20px; }");
            html.AppendLine("        .header { background-color: #2c3e50; color: white; padding: 20px; margin-bottom: 20px; }");
            html.AppendLine("        .header h1 { margin: 0; color: white; }");
            html.AppendLine("        table { border-collapse: collapse; width: 100%; margin-bottom: 20px; }");
            html.AppendLine("        th, td { border: 1px solid #ddd; padding: 10px; text-align: left; white-space: nowrap; }");
            html.AppendLine("        th { background-color: #f2f2f2; position: sticky; top: 0; }");
            html.AppendLine("        tr:nth-child(even) { background-color: #f9f9f9; }");
            html.AppendLine("        tr:hover { background-color: #f1f1f1; }");
            html.AppendLine("        .error { color: #e74c3c; }");
            html.AppendLine("        .success { color: #27ae60; }");
            html.AppendLine("        .warning { color: #f39c12; }");
            html.AppendLine("        .summary-container { display: flex; flex-wrap: wrap; gap: 20px; margin-bottom: 30px; }");
            html.AppendLine("        .summary-section, .parameters-section { flex: 1; min-width: 300px; background: #f8f9fa; padding: 15px; border-radius: 5px; }");
            html.AppendLine("        .ai-analysis { background-color: #f8f9fa; border-left: 4px solid #3498db; padding: 15px; margin: 20px 0; border-radius: 5px; }");
            html.AppendLine("        .ai-analysis h2 { color: #3498db; }");
            html.AppendLine("        .ai-analysis p { white-space: pre-wrap; }");
            html.AppendLine("        .tab { display: none; }");
            html.AppendLine("        .tab-active { display: block; }");
            html.AppendLine("        .tabs { overflow: hidden; border: 1px solid #ccc; background-color: #f1f1f1; display: flex; }");
            html.AppendLine("        .tabs button { background-color: inherit; float: left; border: none; outline: none; cursor: pointer; padding: 14px 16px; transition: 0.3s; font-size: 16px; flex: 1; }");
            html.AppendLine("        .tabs button:hover { background-color: #ddd; }");
            html.AppendLine("        .tabs button.active { background-color: #2c3e50; color: white; }");
            html.AppendLine("        .chart-container { height: 400px; margin: 20px 0; }");
            html.AppendLine("        .metric-card { background: white; border-radius: 5px; padding: 15px; margin-bottom: 15px; box-shadow: 0 2px 4px rgba(0,0,0,0.1); }");
            html.AppendLine("        .metric-card h3 { margin-top: 0; color: #2c3e50; }");
            html.AppendLine("        .metric-value { font-size: 24px; font-weight: bold; margin: 10px 0; }");
            html.AppendLine("        .metric-unit { font-size: 14px; color: #7f8c8d; }");
            html.AppendLine("        .row { display: flex; flex-wrap: wrap; margin: 0 -10px; }");
            html.AppendLine("        .col { flex: 1; padding: 0 10px; min-width: 250px; }");
            html.AppendLine("        @media print {");
            html.AppendLine("            body { font-size: 12px; }");
            html.AppendLine("            .tabs, .tab { display: block !important; }");
            html.AppendLine("            .tab { page-break-after: always; margin-bottom: 30px; }");
            html.AppendLine("            button { display: none; }");
            html.AppendLine("        }");
            html.AppendLine("    </style>");
            html.AppendLine("</head>");
            html.AppendLine("<body>");

            html.AppendLine("    <div class=\"header\">");
            html.AppendLine("        <div class=\"container\">");
            html.AppendLine($"            <h1>Endurance Test Results</h1>");
            html.AppendLine($"            <p>URL: {HtmlEncode(testParameters.Url)}</p>");
            html.AppendLine($"            <p>Test Date: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}</p>");
            html.AppendLine("        </div>");
            html.AppendLine("    </div>");

            html.AppendLine("    <div class=\"container\">");

            html.AppendLine("        <div class=\"tabs\">");
            html.AppendLine("            <button class=\"tablinks active\" onclick=\"openTab(event, 'summary-tab')\">Summary</button>");
            html.AppendLine("            <button class=\"tablinks\" onclick=\"openTab(event, 'charts-tab')\">Charts</button>");
            html.AppendLine("            <button class=\"tablinks\" onclick=\"openTab(event, 'round-data-tab')\">Round Data</button>");
            html.AppendLine("            <button class=\"tablinks\" onclick=\"openTab(event, 'detail-data-tab')\">Detail Data</button>");

            if (!string.IsNullOrWhiteSpace(aiAnalysisResult))
            {
                html.AppendLine("            <button class=\"tablinks\" onclick=\"openTab(event, 'ai-analysis-tab')\">AI Analysis</button>");
            }

            html.AppendLine("        </div>");

            // Summary Tab
            html.AppendLine("        <div id=\"summary-tab\" class=\"tab tab-active\">");
            html.AppendLine("            <h2>Test Summary</h2>");
            html.AppendLine("            <div class=\"row\">");

            html.AppendLine("                <div class=\"col\">");
            html.AppendLine("                    <div class=\"metric-card\">");
            html.AppendLine("                        <h3>Requests</h3>");
            html.AppendLine($"                        <div class=\"metric-value\">{testSummary.TotalRequestsProcessed}</div>");
            html.AppendLine("                        <div class=\"metric-unit\">Total Requests</div>");
            html.AppendLine("                    </div>");
            html.AppendLine("                </div>");

            html.AppendLine("                <div class=\"col\">");
            html.AppendLine("                    <div class=\"metric-card\">");
            html.AppendLine("                        <h3>Success Rate</h3>");
            float successRate = testSummary.TotalRequestsProcessed > 0
                ? (float)testSummary.TotalSuccessfulRequests / testSummary.TotalRequestsProcessed * 100
                : 0;
            html.AppendLine($"                        <div class=\"metric-value\">{successRate:0.00}%</div>");
            html.AppendLine($"                        <div class=\"metric-unit\">{testSummary.TotalSuccessfulRequests} successful, {testSummary.TotalFailedRequests} failed</div>");
            html.AppendLine("                    </div>");
            html.AppendLine("                </div>");

            html.AppendLine("                <div class=\"col\">");
            html.AppendLine("                    <div class=\"metric-card\">");
            html.AppendLine("                        <h3>Average Response Time</h3>");
            html.AppendLine($"                        <div class=\"metric-value\">{testSummary.AverageResponseTime}</div>");
            html.AppendLine("                        <div class=\"metric-unit\">milliseconds</div>");
            html.AppendLine("                    </div>");
            html.AppendLine("                </div>");

            html.AppendLine("                <div class=\"col\">");
            html.AppendLine("                    <div class=\"metric-card\">");
            html.AppendLine("                        <h3>Throughput</h3>");
            html.AppendLine($"                        <div class=\"metric-value\">{testSummary.AverageThroughput}</div>");
            html.AppendLine("                        <div class=\"metric-unit\">requests/second</div>");
            html.AppendLine("                    </div>");
            html.AppendLine("                </div>");

            html.AppendLine("            </div>");

            html.AppendLine("            <div class=\"summary-container\">");

            html.AppendLine("                <div class=\"summary-section\">");
            html.AppendLine("                    <h3>Performance Metrics</h3>");
            html.AppendLine("                    <table>");
            html.AppendLine("                        <tr><th>Metric</th><th>Value</th></tr>");
            html.AppendLine($"                        <tr><td>Average Load Time</td><td>{testSummary.AverageLoadTime} ms</td></tr>");
            html.AppendLine($"                        <tr><td>Average Wait Time</td><td>{testSummary.AverageWaitTime} ms</td></tr>");
            html.AppendLine($"                        <tr><td>Average Response Time</td><td>{testSummary.AverageResponseTime} ms</td></tr>");
            html.AppendLine($"                        <tr><td>Average Throughput</td><td>{testSummary.AverageThroughput} req/sec</td></tr>");
            html.AppendLine($"                        <tr><td>Average Error Rate</td><td>{testSummary.AverageErrorRate}%</td></tr>");
            html.AppendLine($"                        <tr><td>Average Round Duration</td><td>{testSummary.AverageRoundDuration} sec</td></tr>");
            html.AppendLine($"                        <tr><td>Average Computer's CPU Usage</td><td>{testSummary.AverageCpuUsage}%</td></tr>");
            html.AppendLine($"                        <tr><td>Average Computer's RAM Usage</td><td>{testSummary.AverageRamUsage} MB</td></tr>");
            html.AppendLine("                    </table>");
            html.AppendLine("                </div>");

            html.AppendLine("                <div class=\"parameters-section\">");
            html.AppendLine("                    <h3>Test Parameters</h3>");
            html.AppendLine("                    <table>");
            html.AppendLine("                        <tr><th>Parameter</th><th>Value</th></tr>");
            html.AppendLine($"                        <tr><td>URL</td><td>{HtmlEncode(testParameters.Url)}</td></tr>");
            html.AppendLine($"                        <tr><td>Mode</td><td>{testParameters.Mode}</td></tr>");

            if (testParameters.Mode == "Stable")
            {
                html.AppendLine($"                        <tr><td>Number of Requests</td><td>{testParameters.MinRequests}</td></tr>");
            }
            else
            {
                html.AppendLine($"                        <tr><td>Minimum Requests</td><td>{testParameters.MinRequests}</td></tr>");
                html.AppendLine($"                        <tr><td>Maximum Requests</td><td>{testParameters.MaxRequests}</td></tr>");
            }

            html.AppendLine($"                        <tr><td>Timeout Per Round</td><td>{testParameters.TimeoutInSeconds} seconds</td></tr>");
            html.AppendLine($"                        <tr><td>Test Duration</td><td>{testParameters.SelectedTimePeriod}</td></tr>");
            html.AppendLine("                    </table>");
            html.AppendLine("                </div>");
            html.AppendLine("            </div>");
            html.AppendLine("        </div>");

            // Charts Tab
            html.AppendLine("        <div id=\"charts-tab\" class=\"tab\">");
            html.AppendLine("            <h2>Performance Charts</h2>");

            // Response Time Chart
            html.AppendLine("            <div class=\"chart-container\">");
            html.AppendLine("                <canvas id=\"responseTimeChart\"></canvas>");
            html.AppendLine("            </div>");

            // Load Time Chart
            html.AppendLine("            <div class=\"chart-container\">");
            html.AppendLine("                <canvas id=\"loadTimeChart\"></canvas>");
            html.AppendLine("            </div>");

            // Wait Time Chart
            html.AppendLine("            <div class=\"chart-container\">");
            html.AppendLine("                <canvas id=\"waitTimeChart\"></canvas>");
            html.AppendLine("            </div>");

            // Throughput Chart
            html.AppendLine("            <div class=\"chart-container\">");
            html.AppendLine("                <canvas id=\"throughputChart\"></canvas>");
            html.AppendLine("            </div>");

            // CPU Usage Chart
            html.AppendLine("            <div class=\"chart-container\">");
            html.AppendLine("                <canvas id=\"cpuUsageChart\"></canvas>");
            html.AppendLine("            </div>");

            // RAM Usage Chart
            html.AppendLine("            <div class=\"chart-container\">");
            html.AppendLine("                <canvas id=\"ramUsageChart\"></canvas>");
            html.AppendLine("            </div>");

            // Error Rate Chart
            html.AppendLine("            <div class=\"chart-container\">");
            html.AppendLine("                <canvas id=\"errorRateChart\"></canvas>");
            html.AppendLine("            </div>");

            // Requests Per Round Chart
            html.AppendLine("            <div class=\"chart-container\">");
            html.AppendLine("                <canvas id=\"requestsChart\"></canvas>");
            html.AppendLine("            </div>");

            // Success vs Failed Requests Chart
            html.AppendLine("            <div class=\"chart-container\">");
            html.AppendLine("                <canvas id=\"successFailChart\"></canvas>");
            html.AppendLine("            </div>");

            // Round Duration Chart
            html.AppendLine("            <div class=\"chart-container\">");
            html.AppendLine("                <canvas id=\"roundDurationChart\"></canvas>");
            html.AppendLine("            </div>");

            html.AppendLine("        </div>");

            // Round Data Tab
            html.AppendLine("        <div id=\"round-data-tab\" class=\"tab\">");
            html.AppendLine("            <h2>Round Data</h2>");
            html.AppendLine("            <div style=\"overflow-x:auto;\">");
            html.AppendLine("                <table>");
            html.AppendLine("                    <tr>");
            html.AppendLine("                        <th>Round</th>");
            html.AppendLine("                        <th>Requests</th>");
            html.AppendLine("                        <th>Success</th>");
            html.AppendLine("                        <th>Failed</th>");
            html.AppendLine("                        <th>Avg Response (ms)</th>");
            html.AppendLine("                        <th>Throughput (req/s)</th>");
            html.AppendLine("                        <th>Error Rate (%)</th>");
            html.AppendLine("                        <th>Computer's CPU (%)</th>");
            html.AppendLine("                        <th>Computer's RAM (MB)</th>");
            html.AppendLine("                        <th>Round Duration (s)</th>");
            html.AppendLine("                    </tr>");

            var roundGroups = testResults
                .GroupBy(r => r.Round)
                .Select(g => new
                {
                    Round = g.Key,
                    RequestPerRound = g.First().RequestPerRound,
                    SuccessfulRequests = g.Sum(r => r.SuccessfulRequests),
                    FailedRequests = g.Sum(r => r.FailedRequests),
                    AverageResponseTime = g.Average(r => r.AverageResponseTime),
                    Throughput = g.Average(r => r.Throughput),
                    ErrorRate = g.Sum(r => r.FailedRequests) * 100.0 / g.First().RequestPerRound,
                    CpuUsage = g.Average(r => r.CpuUsage),
                    RamUsage = g.Average(r => r.RamUsage),
                    RoundDuration = g.Max(r => r.RoundDuration)
                });

            foreach (var roundData in roundGroups)
            {
                html.AppendLine("                    <tr>");
                html.AppendLine($"                        <td>{roundData.Round}</td>");
                html.AppendLine($"                        <td>{roundData.RequestPerRound}</td>");
                html.AppendLine($"                        <td>{roundData.SuccessfulRequests}</td>");
                html.AppendLine($"                        <td>{roundData.FailedRequests}</td>");
                html.AppendLine($"                        <td>{roundData.AverageResponseTime}</td>");
                html.AppendLine($"                        <td>{roundData.Throughput}</td>");
                html.AppendLine($"                        <td>{roundData.ErrorRate}</td>");
                html.AppendLine($"                        <td>{roundData.CpuUsage}</td>");
                html.AppendLine($"                        <td>{roundData.RamUsage}</td>");
                html.AppendLine($"                        <td>{roundData.RoundDuration}</td>");
                html.AppendLine("                    </tr>");
            }

            html.AppendLine("                </table>");
            html.AppendLine("            </div>");
            html.AppendLine("        </div>");

            // Detail Data Tab
            html.AppendLine("        <div id=\"detail-data-tab\" class=\"tab\">");
            html.AppendLine("            <h2>Detailed Request Data</h2>");
            html.AppendLine("            <div style=\"overflow-x:auto;\">");
            html.AppendLine("                <table>");
            html.AppendLine("                    <tr>");
            html.AppendLine("                        <th>Round</th>");
            html.AppendLine("                        <th>Status</th>");
            html.AppendLine("                        <th>Reason</th>");
            html.AppendLine("                        <th>Load Time (ms)</th>");
            html.AppendLine("                        <th>Wait Time (ms)</th>");
            html.AppendLine("                        <th>Response Time (ms)</th>");
            html.AppendLine("                        <th>Computer's CPU (%)</th>");
            html.AppendLine("                        <th>Computer's RAM (MB)</th>");
            html.AppendLine("                        <th>Requests</th>");
            html.AppendLine("                        <th>Success</th>");
            html.AppendLine("                        <th>Failed</th>");
            html.AppendLine("                        <th>Avg Load (ms)</th>");
            html.AppendLine("                        <th>Avg Wait (ms)</th>");
            html.AppendLine("                        <th>Avg Response (ms)</th>");
            html.AppendLine("                        <th>Throughput (req/s)</th>");
            html.AppendLine("                        <th>Error Rate (%)</th>");
            html.AppendLine("                        <th>Round Duration (s)</th>");
            html.AppendLine("                    </tr>");

            foreach (var result in testResults)
            {
                bool isError = result.StatusCode != System.Net.HttpStatusCode.OK;
                string statusClass = isError ? " class=\"error\"" : "";

                html.AppendLine("                    <tr>");
                html.AppendLine($"                        <td>{result.Round}</td>");
                html.AppendLine($"                        <td{statusClass}>{(int)result.StatusCode}</td>");
                html.AppendLine($"                        <td{statusClass}>{HtmlEncode(result.ReasonPhrase)}</td>");
                html.AppendLine($"                        <td>{result.LoadTime.TotalMilliseconds}</td>");
                html.AppendLine($"                        <td>{result.WaitTime.TotalMilliseconds}</td>");
                html.AppendLine($"                        <td>{result.ResponseTime.TotalMilliseconds}</td>");
                html.AppendLine($"                        <td>{result.CpuUsage}</td>");
                html.AppendLine($"                        <td>{result.RamUsage}</td>");
                html.AppendLine($"                        <td>{result.RequestPerRound}</td>");
                html.AppendLine($"                        <td>{result.SuccessfulRequests}</td>");
                html.AppendLine($"                        <td>{result.FailedRequests}</td>");
                html.AppendLine($"                        <td>{result.AverageLoadTime}</td>");
                html.AppendLine($"                        <td>{result.AverageWaitTime}</td>");
                html.AppendLine($"                        <td>{result.AverageResponseTime}</td>");
                html.AppendLine($"                        <td>{result.Throughput}</td>");
                html.AppendLine($"                        <td>{result.ErrorRate}</td>");
                html.AppendLine($"                        <td>{result.RoundDuration}</td>");
                html.AppendLine("                    </tr>");
            }

            html.AppendLine("                </table>");
            html.AppendLine("            </div>");
            html.AppendLine("        </div>");

            // AI Analysis Tab (if available)
            if (!string.IsNullOrWhiteSpace(aiAnalysisResult))
            {
                html.AppendLine("        <div id=\"ai-analysis-tab\" class=\"tab\">");
                html.AppendLine("            <h2>AI Performance Analysis</h2>");
                html.AppendLine("            <div class=\"ai-analysis\">");
                html.AppendLine($"                <p>{HtmlEncode(aiAnalysisResult).Replace("\n", "<br>")}</p>");
                html.AppendLine("            </div>");
                html.AppendLine("        </div>");
            }

            // JavaScript for tabs and charts
            html.AppendLine("        <script src=\"https://cdn.jsdelivr.net/npm/chart.js\"></script>");
            html.AppendLine("        <script>");
            html.AppendLine("            function openTab(evt, tabName) {");
            html.AppendLine("                var i, tabcontent, tablinks;");
            html.AppendLine("                tabcontent = document.getElementsByClassName(\"tab\");");
            html.AppendLine("                for (i = 0; i < tabcontent.length; i++) {");
            html.AppendLine("                    tabcontent[i].className = tabcontent[i].className.replace(\" tab-active\", \"\");");
            html.AppendLine("                }");
            html.AppendLine("                tablinks = document.getElementsByClassName(\"tablinks\");");
            html.AppendLine("                for (i = 0; i < tablinks.length; i++) {");
            html.AppendLine("                    tablinks[i].className = tablinks[i].className.replace(\" active\", \"\");");
            html.AppendLine("                }");
            html.AppendLine("                document.getElementById(tabName).className += \" tab-active\";");
            html.AppendLine("                evt.currentTarget.className += \" active\";");
            html.AppendLine("            }");
            html.AppendLine("            ");

            // Chart.js data preparation
            html.AppendLine("            // Chart data");
            html.AppendLine("            document.addEventListener('DOMContentLoaded', function() {");

            // Prepare data for charts
            html.AppendLine("                const rounds = [" + string.Join(", ", testResults.Select(r => r.Round)) + "];");
            html.AppendLine("                const responseTime = [" + string.Join(", ", testResults.Select(r => r.AverageResponseTime.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture))) + "];");
            html.AppendLine("                const loadTime = [" + string.Join(", ", testResults.Select(r => r.AverageLoadTime.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture))) + "];");
            html.AppendLine("                const waitTime = [" + string.Join(", ", testResults.Select(r => r.AverageWaitTime.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture))) + "];");
            html.AppendLine("                const throughput = [" + string.Join(", ", testResults.Select(r => r.Throughput.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture))) + "];");
            html.AppendLine("                const cpuUsage = [" + string.Join(", ", testResults.Select(r => r.CpuUsage.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture))) + "];");
            html.AppendLine("                const ramUsage = [" + string.Join(", ", testResults.Select(r => r.RamUsage.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture))) + "];");
            html.AppendLine("                const errorRate = [" + string.Join(", ", testResults.Select(r => r.ErrorRate.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture))) + "];");
            html.AppendLine("                const requestsPerRound = [" + string.Join(", ", testResults.Select(r => r.RequestPerRound)) + "];");
            html.AppendLine("                const successfulRequests = [" + string.Join(", ", testResults.Select(r => r.SuccessfulRequests)) + "];");
            html.AppendLine("                const failedRequests = [" + string.Join(", ", testResults.Select(r => r.FailedRequests)) + "];");
            html.AppendLine("                const roundDuration = [" + string.Join(", ", testResults.Select(r => r.RoundDuration.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture))) + "];");

            // Response Time Chart
            html.AppendLine("                // Response Time Chart");
            html.AppendLine("                const responseTimeCtx = document.getElementById('responseTimeChart');");
            html.AppendLine("                new Chart(responseTimeCtx, {");
            html.AppendLine("                    type: 'line',");
            html.AppendLine("                    data: {");
            html.AppendLine("                        labels: rounds,");
            html.AppendLine("                        datasets: [{");
            html.AppendLine("                            label: 'Response Time (ms)',");
            html.AppendLine("                            data: responseTime,");
            html.AppendLine("                            borderColor: 'rgb(75, 192, 192)',");
            html.AppendLine("                            backgroundColor: 'rgba(75, 192, 192, 0.2)',");
            html.AppendLine("                            tension: 0.1,");
            html.AppendLine("                            fill: true");
            html.AppendLine("                        }]");
            html.AppendLine("                    },");
            html.AppendLine("                    options: {");
            html.AppendLine("                        responsive: true,");
            html.AppendLine("                        plugins: {");
            html.AppendLine("                            title: {");
            html.AppendLine("                                display: true,");
            html.AppendLine("                                text: 'Response Time Analysis'");
            html.AppendLine("                            }");
            html.AppendLine("                        }");
            html.AppendLine("                    }");
            html.AppendLine("                });");

            // Load Time Chart
            html.AppendLine("                // Load Time Chart");
            html.AppendLine("                const loadTimeCtx = document.getElementById('loadTimeChart');");
            html.AppendLine("                new Chart(loadTimeCtx, {");
            html.AppendLine("                    type: 'line',");
            html.AppendLine("                    data: {");
            html.AppendLine("                        labels: rounds,");
            html.AppendLine("                        datasets: [{");
            html.AppendLine("                            label: 'Load Time (ms)',");
            html.AppendLine("                            data: loadTime,");
            html.AppendLine("                            borderColor: 'rgb(255, 159, 64)',");
            html.AppendLine("                            backgroundColor: 'rgba(255, 159, 64, 0.2)',");
            html.AppendLine("                            tension: 0.1,");
            html.AppendLine("                            fill: true");
            html.AppendLine("                        }]");
            html.AppendLine("                    },");
            html.AppendLine("                    options: {");
            html.AppendLine("                        responsive: true,");
            html.AppendLine("                        plugins: {");
            html.AppendLine("                            title: {");
            html.AppendLine("                                display: true,");
            html.AppendLine("                                text: 'Load Time Analysis'");
            html.AppendLine("                            }");
            html.AppendLine("                        }");
            html.AppendLine("                    }");
            html.AppendLine("                });");

            // Wait Time Chart
            html.AppendLine("                // Wait Time Chart");
            html.AppendLine("                const waitTimeCtx = document.getElementById('waitTimeChart');");
            html.AppendLine("                new Chart(waitTimeCtx, {");
            html.AppendLine("                    type: 'line',");
            html.AppendLine("                    data: {");
            html.AppendLine("                        labels: rounds,");
            html.AppendLine("                        datasets: [{");
            html.AppendLine("                            label: 'Wait Time (ms)',");
            html.AppendLine("                            data: waitTime,");
            html.AppendLine("                            borderColor: 'rgb(153, 102, 255)',");
            html.AppendLine("                            backgroundColor: 'rgba(153, 102, 255, 0.2)',");
            html.AppendLine("                            tension: 0.1,");
            html.AppendLine("                            fill: true");
            html.AppendLine("                        }]");
            html.AppendLine("                    },");
            html.AppendLine("                    options: {");
            html.AppendLine("                        responsive: true,");
            html.AppendLine("                        plugins: {");
            html.AppendLine("                            title: {");
            html.AppendLine("                                display: true,");
            html.AppendLine("                                text: 'Wait Time Analysis'");
            html.AppendLine("                            }");
            html.AppendLine("                        }");
            html.AppendLine("                    }");
            html.AppendLine("                });");

            // Throughput Chart
            html.AppendLine("                // Throughput Chart");
            html.AppendLine("                const throughputCtx = document.getElementById('throughputChart');");
            html.AppendLine("                new Chart(throughputCtx, {");
            html.AppendLine("                    type: 'bar',");
            html.AppendLine("                    data: {");
            html.AppendLine("                        labels: rounds,");
            html.AppendLine("                        datasets: [{");
            html.AppendLine("                            label: 'Throughput (req/sec)',");
            html.AppendLine("                            data: throughput,");
            html.AppendLine("                            backgroundColor: 'rgba(54, 162, 235, 0.5)',");
            html.AppendLine("                            borderColor: 'rgb(54, 162, 235)',");
            html.AppendLine("                            borderWidth: 1");
            html.AppendLine("                        }]");
            html.AppendLine("                    },");
            html.AppendLine("                    options: {");
            html.AppendLine("                        responsive: true,");
            html.AppendLine("                        plugins: {");
            html.AppendLine("                            title: {");
            html.AppendLine("                                display: true,");
            html.AppendLine("                                text: 'Throughput Analysis'");
            html.AppendLine("                            }");
            html.AppendLine("                        }");
            html.AppendLine("                    }");
            html.AppendLine("                });");

            // CPU Usage Chart
            html.AppendLine("                // CPU Usage Chart");
            html.AppendLine("                const cpuUsageCtx = document.getElementById('cpuUsageChart');");
            html.AppendLine("                new Chart(cpuUsageCtx, {");
            html.AppendLine("                    type: 'line',");
            html.AppendLine("                    data: {");
            html.AppendLine("                        labels: rounds,");
            html.AppendLine("                        datasets: [{");
            html.AppendLine("                            label: 'Computer's CPU Usage (%)',");
            html.AppendLine("                            data: cpuUsage,");
            html.AppendLine("                            borderColor: 'rgb(255, 99, 132)',");
            html.AppendLine("                            backgroundColor: 'rgba(255, 99, 132, 0.2)',");
            html.AppendLine("                            tension: 0.1,");
            html.AppendLine("                            fill: true");
            html.AppendLine("                        }]");
            html.AppendLine("                    },");
            html.AppendLine("                    options: {");
            html.AppendLine("                        responsive: true,");
            html.AppendLine("                        plugins: {");
            html.AppendLine("                            title: {");
            html.AppendLine("                                display: true,");
            html.AppendLine("                                text: 'Computer's CPU Usage Analysis'");
            html.AppendLine("                            }");
            html.AppendLine("                        }");
            html.AppendLine("                    }");
            html.AppendLine("                });");

            // RAM Usage Chart
            html.AppendLine("                // RAM Usage Chart");
            html.AppendLine("                const ramUsageCtx = document.getElementById('ramUsageChart');");
            html.AppendLine("                new Chart(ramUsageCtx, {");
            html.AppendLine("                    type: 'line',");
            html.AppendLine("                    data: {");
            html.AppendLine("                        labels: rounds,");
            html.AppendLine("                        datasets: [{");
            html.AppendLine("                            label: 'Computer's RAM Usage (MB)',");
            html.AppendLine("                            data: ramUsage,");
            html.AppendLine("                            borderColor: 'rgb(255, 205, 86)',");
            html.AppendLine("                            backgroundColor: 'rgba(255, 205, 86, 0.2)',");
            html.AppendLine("                            tension: 0.1,");
            html.AppendLine("                            fill: true");
            html.AppendLine("                        }]");
            html.AppendLine("                    },");
            html.AppendLine("                    options: {");
            html.AppendLine("                        responsive: true,");
            html.AppendLine("                        plugins: {");
            html.AppendLine("                            title: {");
            html.AppendLine("                                display: true,");
            html.AppendLine("                                text: 'Computer's RAM Usage Analysis'");
            html.AppendLine("                            }");
            html.AppendLine("                        }");
            html.AppendLine("                    }");
            html.AppendLine("                });");

            // Error Rate Chart
            html.AppendLine("                // Error Rate Chart");
            html.AppendLine("                const errorRateCtx = document.getElementById('errorRateChart');");
            html.AppendLine("                new Chart(errorRateCtx, {");
            html.AppendLine("                    type: 'line',");
            html.AppendLine("                    data: {");
            html.AppendLine("                        labels: rounds,");
            html.AppendLine("                        datasets: [{");
            html.AppendLine("                            label: 'Error Rate (%)',");
            html.AppendLine("                            data: errorRate,");
            html.AppendLine("                            borderColor: 'rgb(255, 99, 132)',");
            html.AppendLine("                            backgroundColor: 'rgba(255, 99, 132, 0.2)',");
            html.AppendLine("                            tension: 0.1,");
            html.AppendLine("                            fill: true");
            html.AppendLine("                        }]");
            html.AppendLine("                    },");
            html.AppendLine("                    options: {");
            html.AppendLine("                        responsive: true,");
            html.AppendLine("                        plugins: {");
            html.AppendLine("                            title: {");
            html.AppendLine("                                display: true,");
            html.AppendLine("                                text: 'Error Rate Analysis'");
            html.AppendLine("                            }");
            html.AppendLine("                        }");
            html.AppendLine("                    }");
            html.AppendLine("                });");

            // Requests Per Round Chart
            html.AppendLine("                // Requests Per Round Chart");
            html.AppendLine("                const requestsCtx = document.getElementById('requestsChart');");
            html.AppendLine("                new Chart(requestsCtx, {");
            html.AppendLine("                    type: 'bar',");
            html.AppendLine("                    data: {");
            html.AppendLine("                        labels: rounds,");
            html.AppendLine("                        datasets: [{");
            html.AppendLine("                            label: 'Requests Per Round',");
            html.AppendLine("                            data: requestsPerRound,");
            html.AppendLine("                            backgroundColor: 'rgba(54, 162, 235, 0.5)',");
            html.AppendLine("                            borderColor: 'rgb(54, 162, 235)',");
            html.AppendLine("                            borderWidth: 1");
            html.AppendLine("                        }]");
            html.AppendLine("                    },");
            html.AppendLine("                    options: {");
            html.AppendLine("                        responsive: true,");
            html.AppendLine("                        plugins: {");
            html.AppendLine("                            title: {");
            html.AppendLine("                                display: true,");
            html.AppendLine("                                text: 'Requests Per Round'");
            html.AppendLine("                            }");
            html.AppendLine("                        }");
            html.AppendLine("                    }");
            html.AppendLine("                });");

            // Success vs Failed Requests Chart
            html.AppendLine("                // Success vs Failed Requests Chart");
            html.AppendLine("                const successFailCtx = document.getElementById('successFailChart');");
            html.AppendLine("                new Chart(successFailCtx, {");
            html.AppendLine("                    type: 'bar',");
            html.AppendLine("                    data: {");
            html.AppendLine("                        labels: rounds,");
            html.AppendLine("                        datasets: [{");
            html.AppendLine("                            label: 'Successful Requests',");
            html.AppendLine("                            data: successfulRequests,");
            html.AppendLine("                            backgroundColor: 'rgba(75, 192, 192, 0.5)',");
            html.AppendLine("                            borderColor: 'rgb(75, 192, 192)',");
            html.AppendLine("                            borderWidth: 1");
            html.AppendLine("                        }, {");
            html.AppendLine("                            label: 'Failed Requests',");
            html.AppendLine("                            data: failedRequests,");
            html.AppendLine("                            backgroundColor: 'rgba(255, 99, 132, 0.5)',");
            html.AppendLine("                            borderColor: 'rgb(255, 99, 132)',");
            html.AppendLine("                            borderWidth: 1");
            html.AppendLine("                        }]");
            html.AppendLine("                    },");
            html.AppendLine("                    options: {");
            html.AppendLine("                        responsive: true,");
            html.AppendLine("                        plugins: {");
            html.AppendLine("                            title: {");
            html.AppendLine("                                display: true,");
            html.AppendLine("                                text: 'Successful vs Failed Requests'");
            html.AppendLine("                            }");
            html.AppendLine("                        },");
            html.AppendLine("                        scales: {");
            html.AppendLine("                            x: {");
            html.AppendLine("                                stacked: false");
            html.AppendLine("                            },");
            html.AppendLine("                            y: {");
            html.AppendLine("                                stacked: false");
            html.AppendLine("                            }");
            html.AppendLine("                        }");
            html.AppendLine("                    }");
            html.AppendLine("                });");

            // Round Duration Chart
            html.AppendLine("                // Round Duration Chart");
            html.AppendLine("                const roundDurationCtx = document.getElementById('roundDurationChart');");
            html.AppendLine("                new Chart(roundDurationCtx, {");
            html.AppendLine("                    type: 'line',");
            html.AppendLine("                    data: {");
            html.AppendLine("                        labels: rounds,");
            html.AppendLine("                        datasets: [{");
            html.AppendLine("                            label: 'Round Duration (sec)',");
            html.AppendLine("                            data: roundDuration,");
            html.AppendLine("                            borderColor: 'rgb(153, 102, 255)',");
            html.AppendLine("                            backgroundColor: 'rgba(153, 102, 255, 0.2)',");
            html.AppendLine("                            tension: 0.1,");
            html.AppendLine("                            fill: true");
            html.AppendLine("                        }]");
            html.AppendLine("                    },");
            html.AppendLine("                    options: {");
            html.AppendLine("                        responsive: true,");
            html.AppendLine("                        plugins: {");
            html.AppendLine("                            title: {");
            html.AppendLine("                                display: true,");
            html.AppendLine("                                text: 'Round Duration Analysis'");
            html.AppendLine("                            }");
            html.AppendLine("                        }");
            html.AppendLine("                    }");
            html.AppendLine("                });");
            html.AppendLine("            });");
            html.AppendLine("        </script>");

            // Footer
            html.AppendLine("        <p style=\"text-align: center; margin-top: 30px; color: #777;\">Generated with Endurance Testing Tool by Rahma Bintang Pratama on " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "</p>");
            html.AppendLine("    </div>");
            html.AppendLine("</body>");
            html.AppendLine("</html>");

            return html.ToString();
        }

        private string HtmlEncode(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            return input
                .Replace("&", "&amp;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;")
                .Replace("\"", "&quot;")
                .Replace("'", "&#39;");
        }
    }
}