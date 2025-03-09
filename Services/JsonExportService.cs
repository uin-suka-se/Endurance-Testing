using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Forms;
using Endurance_Testing.Core;
using Endurance_Testing.Models;

namespace Endurance_Testing.Services
{
    public class JsonExportService
    {
        public bool ExportToJson(List<EnduranceTestResult> testResults, TestSummary testSummary,
                               TestParameters testParameters, string aiAnalysisResult = "")
        {
            try
            {
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = "JSON Files|*.json";
                    saveFileDialog.Title = "Save a JSON File";
                    saveFileDialog.FileName = "EnduranceTestResults.json";

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        var exportData = new JsonExportData
                        {
                            Metadata = new ExportMetadata
                            {
                                ExportDate = DateTime.UtcNow,
                                Url = testParameters.Url
                            },
                            TestParameters = testParameters,
                            TestSummary = testSummary,
                            TestResults = testResults,
                            AIAnalysis = aiAnalysisResult
                        };

                        var options = new JsonSerializerOptions
                        {
                            WriteIndented = true,
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                        };

                        string jsonString = JsonSerializer.Serialize(exportData, options);
                        File.WriteAllText(saveFileDialog.FileName, jsonString);

                        MessageBox.Show("JSON export successful!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to export to JSON: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return false;
        }
    }

    public class JsonExportData
    {
        public ExportMetadata Metadata { get; set; }
        public TestParameters TestParameters { get; set; }
        public TestSummary TestSummary { get; set; }
        public List<EnduranceTestResult> TestResults { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string AIAnalysis { get; set; }
    }

    public class ExportMetadata
    {
        public DateTime ExportDate { get; set; }
        public string Url { get; set; }
    }
}