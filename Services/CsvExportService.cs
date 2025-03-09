using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Endurance_Testing.Core;
using Endurance_Testing.Models;

namespace Endurance_Testing.Services
{
    public class CsvExportService
    {
        public bool ExportToCsv(List<EnduranceTestResult> testResults, TestSummary testSummary,
                               TestParameters testParameters, string aiAnalysisResult = "")
        {
            try
            {
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = "CSV Files|*.csv";
                    saveFileDialog.Title = "Save a CSV File";
                    saveFileDialog.FileName = "EnduranceTestResults.csv";

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        CultureInfo culture = CultureInfo.InvariantCulture;

                        using (StreamWriter writer = new StreamWriter(saveFileDialog.FileName, false, Encoding.UTF8))
                        {
                            writer.WriteLine($"Endurance Testing Results from URL: {testParameters.Url}");
                            writer.WriteLine($"Export Date: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}");
                            writer.WriteLine();

                            writer.WriteLine("Round,Status,Reason,LoadTime (ms),WaitTime (ms),ResponseTime (ms),CPU Usage (%),RAM Usage (MB),Total Requests,Successful Requests,Failed Requests,Average Load Time (ms),Average Wait Time (ms),Average Response Time (ms),Throughput (req/sec),Error Rate (%),Round Duration (sec)");

                            foreach (var result in testResults)
                            {
                                writer.WriteLine(
                                    $"{result.Round}," +
                                    $"{(int)result.StatusCode}," +
                                    $"\"{EscapeCsvField(result.ReasonPhrase)}\"," +
                                    $"{result.LoadTime.TotalMilliseconds.ToString(culture)}," +
                                    $"{result.WaitTime.TotalMilliseconds.ToString(culture)}," +
                                    $"{result.ResponseTime.TotalMilliseconds.ToString(culture)}," +
                                    $"{result.CpuUsage.ToString(culture)}," +
                                    $"{result.RamUsage.ToString(culture)}," +
                                    $"{result.RequestPerRound}," +
                                    $"{result.SuccessfulRequests}," +
                                    $"{result.FailedRequests}," +
                                    $"{result.AverageLoadTime.ToString(culture)}," +
                                    $"{result.AverageWaitTime.ToString(culture)}," +
                                    $"{result.AverageResponseTime.ToString(culture)}," +
                                    $"{result.Throughput.ToString(culture)}," +
                                    $"{result.ErrorRate.ToString(culture)}," +
                                    $"{result.RoundDuration.ToString(culture)}"
                                );
                            }

                            writer.WriteLine();
                            writer.WriteLine();

                            writer.WriteLine("SUMMARY");
                            writer.WriteLine($"Total Requests,{testSummary.TotalRequestsProcessed}");
                            writer.WriteLine($"Successful Requests,{testSummary.TotalSuccessfulRequests}");
                            writer.WriteLine($"Failed Requests,{testSummary.TotalFailedRequests}");
                            writer.WriteLine($"Average CPU Usage (%),{testSummary.AverageCpuUsage.ToString(culture)}");
                            writer.WriteLine($"Average RAM Usage (MB),{testSummary.AverageRamUsage.ToString(culture)}");
                            writer.WriteLine($"Average Load Time (ms),{testSummary.AverageLoadTime.ToString(culture)}");
                            writer.WriteLine($"Average Wait Time (ms),{testSummary.AverageWaitTime.ToString(culture)}");
                            writer.WriteLine($"Average Response Time (ms),{testSummary.AverageResponseTime.ToString(culture)}");
                            writer.WriteLine($"Average Throughput (req/sec),{testSummary.AverageThroughput.ToString(culture)}");
                            writer.WriteLine($"Average Error Rate (%),{testSummary.AverageErrorRate.ToString(culture)}");
                            writer.WriteLine($"Average Round Duration (sec),{testSummary.AverageRoundDuration.ToString(culture)}");

                            writer.WriteLine();
                            writer.WriteLine();

                            writer.WriteLine("TEST PARAMETERS");
                            writer.WriteLine($"URL,{testParameters.Url}");
                            writer.WriteLine($"Mode,{testParameters.Mode}");

                            if (testParameters.Mode == "Stable")
                            {
                                writer.WriteLine($"Number of Requests,{testParameters.MinRequests}");
                            }
                            else
                            {
                                writer.WriteLine($"Minimum Requests,{testParameters.MinRequests}");
                                writer.WriteLine($"Maximum Requests,{testParameters.MaxRequests}");
                            }

                            writer.WriteLine($"Timeout Per Round,{testParameters.TimeoutInSeconds} seconds");
                            writer.WriteLine($"Test Duration,{testParameters.SelectedTimePeriod}");

                            if (!string.IsNullOrWhiteSpace(aiAnalysisResult))
                            {
                                writer.WriteLine();
                                writer.WriteLine();
                                writer.WriteLine("AI ANALYSIS");

                                string[] analysisLines = aiAnalysisResult.Split(
                                    new[] { '\r', '\n' },
                                    StringSplitOptions.RemoveEmptyEntries
                                );

                                foreach (var line in analysisLines)
                                {
                                    writer.WriteLine($"\"{EscapeCsvField(line)}\"");
                                }
                            }
                        }

                        MessageBox.Show("Export successful!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to export to CSV: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return false;
        }

        private string EscapeCsvField(string field)
        {
            if (string.IsNullOrEmpty(field))
                return string.Empty;

            return field.Replace("\"", "\"\"");
        }
    }
}