using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ClosedXML.Excel;

using Endurance_Testing.Models;

namespace Endurance_Testing.Services
{
    public class ExcelExportService
    {
        public bool ExportToExcel(List<EnduranceTestResult> testResults, TestSummary testSummary, TestParameters testParameters, string aiAnalysisResult = "")
        {
            try
            {
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Endurance Test Results");

                    worksheet.Cell(1, 1).Value = $"Endurance Testing Results from URL: {testParameters.Url}";
                    worksheet.Cell(1, 1).Style.Font.Bold = true;
                    worksheet.Cell(1, 1).Style.Font.FontSize = 14;
                    worksheet.Cell(1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Range("A1:Q1").Merge();

                    SetupColumnHeaders(worksheet);

                    worksheet.SheetView.Freeze(2, 1);

                    PopulateTestResults(worksheet, testResults);

                    AddSummarySection(worksheet, testSummary);

                    int paramLastColumn = AddParametersSection(worksheet, testParameters);

                    if (!string.IsNullOrEmpty(aiAnalysisResult))
                    {
                        AddAiAnalysisSection(worksheet, aiAnalysisResult, paramLastColumn + 2);
                    }

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
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to export to Excel: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return false;
        }

        private void SetupColumnHeaders(IXLWorksheet worksheet)
        {
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

            var headerRange = worksheet.Range(2, 1, 2, 17);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
        }

        private void PopulateTestResults(IXLWorksheet worksheet, List<EnduranceTestResult> testResults)
        {
            int row = 3;
            foreach (var result in testResults)
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

                if (result.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    worksheet.Cell(row, 2).Style.Font.FontColor = XLColor.Red;
                }

                row++;
            }
        }

        private void AddSummarySection(IXLWorksheet worksheet, TestSummary testSummary)
        {
            int summaryStartRow = 1;
            int summaryStartColumn = 19;

            worksheet.Cell(summaryStartRow, summaryStartColumn).Value = "Overall Summary";
            worksheet.Cell(summaryStartRow, summaryStartColumn).Style.Font.Bold = true;
            worksheet.Cell(summaryStartRow, summaryStartColumn).Style.Font.FontSize = 14;
            worksheet.Cell(summaryStartRow, summaryStartColumn).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Range(summaryStartRow, summaryStartColumn, summaryStartRow, summaryStartColumn + 1).Merge();

            AddSummaryRow(worksheet, ref summaryStartRow, summaryStartColumn, "Total Requests", testSummary.TotalRequestsProcessed);
            AddSummaryRow(worksheet, ref summaryStartRow, summaryStartColumn, "Successful Requests", testSummary.TotalSuccessfulRequests);
            AddSummaryRow(worksheet, ref summaryStartRow, summaryStartColumn, "Failed Requests", testSummary.TotalFailedRequests);
            AddSummaryRow(worksheet, ref summaryStartRow, summaryStartColumn, "Average Computer's CPU Usage", $"{testSummary.AverageCpuUsage}%");
            AddSummaryRow(worksheet, ref summaryStartRow, summaryStartColumn, "Average Computer's RAM Usage", $"{testSummary.AverageRamUsage} MB");
            AddSummaryRow(worksheet, ref summaryStartRow, summaryStartColumn, "Average Load Time", $"{testSummary.AverageLoadTime} ms");
            AddSummaryRow(worksheet, ref summaryStartRow, summaryStartColumn, "Average Wait Time", $"{testSummary.AverageWaitTime} ms");
            AddSummaryRow(worksheet, ref summaryStartRow, summaryStartColumn, "Average Response Time", $"{testSummary.AverageResponseTime} ms");
            AddSummaryRow(worksheet, ref summaryStartRow, summaryStartColumn, "Average Throughput", $"{testSummary.AverageThroughput} requests/second");
            AddSummaryRow(worksheet, ref summaryStartRow, summaryStartColumn, "Average Error Rate", $"{testSummary.AverageErrorRate}%");
            AddSummaryRow(worksheet, ref summaryStartRow, summaryStartColumn, "Average Round Duration", $"{testSummary.AverageRoundDuration} seconds");
        }

        private void AddSummaryRow(IXLWorksheet worksheet, ref int row, int column, string label, object value)
        {
            row++;
            worksheet.Cell(row, column).Value = label;
            worksheet.Cell(row, column).Style.Font.Bold = true;
            worksheet.Cell(row, column + 1).Value = XLCellValue.FromObject(value);
        }

        private int AddParametersSection(IXLWorksheet worksheet, TestParameters testParameters)
        {
            int paramStartRow = 1;
            int paramStartColumn = 22;

            worksheet.Cell(paramStartRow, paramStartColumn).Value = "Test Parameter";
            worksheet.Cell(paramStartRow, paramStartColumn).Style.Font.Bold = true;
            worksheet.Cell(paramStartRow, paramStartColumn).Style.Font.FontSize = 14;
            worksheet.Cell(paramStartRow, paramStartColumn).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Range(paramStartRow, paramStartColumn, paramStartRow, paramStartColumn + 1).Merge();

            paramStartRow++;
            worksheet.Cell(paramStartRow, paramStartColumn).Value = "URL:";
            worksheet.Cell(paramStartRow, paramStartColumn + 1).Value = testParameters.Url;

            string selectedMode = testParameters.Mode;

            switch (selectedMode)
            {
                case "Stable":
                    paramStartRow++;
                    worksheet.Cell(paramStartRow, paramStartColumn).Value = "Number of Requests:";
                    worksheet.Cell(paramStartRow, paramStartColumn + 1).Value = testParameters.MinRequests;
                    break;

                default:
                    paramStartRow++;
                    worksheet.Cell(paramStartRow, paramStartColumn).Value = "Number of Requests (Min):";
                    worksheet.Cell(paramStartRow, paramStartColumn + 1).Value = testParameters.MinRequests;

                    paramStartRow++;
                    worksheet.Cell(paramStartRow, paramStartColumn).Value = "Number of Requests (Max):";
                    worksheet.Cell(paramStartRow, paramStartColumn + 1).Value = testParameters.MaxRequests;
                    break;
            }

            paramStartRow++;
            worksheet.Cell(paramStartRow, paramStartColumn).Value = "Mode:";
            worksheet.Cell(paramStartRow, paramStartColumn + 1).Value = selectedMode;

            paramStartRow++;
            worksheet.Cell(paramStartRow, paramStartColumn).Value = "Timeout Per Round:";
            worksheet.Cell(paramStartRow, paramStartColumn + 1).Value = testParameters.TimeoutInSeconds + " second(s)";

            paramStartRow++;
            worksheet.Cell(paramStartRow, paramStartColumn).Value = "Time in Period:";
            worksheet.Cell(paramStartRow, paramStartColumn + 1).Value = testParameters.SelectedTimePeriod;

            return paramStartColumn + 1;
        }

        private void AddAiAnalysisSection(IXLWorksheet worksheet, string aiAnalysisResult, int startColumn)
        {
            int startRow = 1;

            worksheet.Cell(startRow, startColumn).Value = "AI Analysis Results";
            worksheet.Cell(startRow, startColumn).Style.Font.Bold = true;
            worksheet.Cell(startRow, startColumn).Style.Font.FontSize = 14;
            worksheet.Cell(startRow, startColumn).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            worksheet.Range(startRow, startColumn, startRow, startColumn + 6).Merge();

            startRow += 2;

            for (int i = 0; i <= 6; i++)
            {
                worksheet.Column(startColumn + i).Width = 40;
            }

            worksheet.Cell(startRow, startColumn).Value = aiAnalysisResult;
            var cell = worksheet.Cell(startRow, startColumn);
            cell.Style.Alignment.WrapText = true;
            cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

            var mergedRange = worksheet.Range(startRow, startColumn, startRow + 49, startColumn + 6);
            mergedRange.Merge();

            mergedRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            mergedRange.Style.Border.OutsideBorderColor = XLColor.Gray;

            cell.Style.Font.FontSize = 11;
        }
    }
}