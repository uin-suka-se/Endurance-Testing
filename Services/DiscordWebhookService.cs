using Endurance_Testing.Core;
using Endurance_Testing.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Endurance_Testing.Services
{
    public class DiscordWebhookService
    {
        private readonly HttpClient _httpClient;

        public DiscordWebhookService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<bool> SendToDiscord(string webhookUrl, List<EnduranceTestResult> enduranceTestResults,
            TestSummary summary, TestParameters parameters, string aiAnalysisResult = null)
        {
            try
            {
                if (string.IsNullOrEmpty(webhookUrl))
                {
                    throw new ArgumentException("Discord webhook URL is required");
                }

                var message = CreateDiscordMessage(summary, parameters, aiAnalysisResult);

                var jsonContent = JsonSerializer.Serialize(message);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(webhookUrl, content);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending to Discord: {ex.Message}");
                return false;
            }
        }

        private object CreateDiscordMessage(TestSummary summary, TestParameters parameters, string aiAnalysisResult)
        {
            double errorRate = summary.AverageErrorRate;

            int color = errorRate < 10 ? 0x57F287 :
                        errorRate < 30 ? 0xFEE75C :
                                         0xED4245;

            var embed = new
            {
                title = $"Endurance Test Results for {parameters.Url}",
                color = color,
                fields = new List<object>
                {
                    // Test Parameters
                    new { name = "📋 Test Parameters", value = GetParametersText(parameters), inline = false },
                    
                    // Request Statistics
                    new { name = "📈 Total Requests", value = summary.TotalRequestsProcessed.ToString(), inline = true },
                    new { name = "✅ Successful Requests", value = summary.TotalSuccessfulRequests.ToString(), inline = true },
                    new { name = "❌ Failed Requests", value = summary.TotalFailedRequests.ToString(), inline = true },
                    new { name = "🔄 Error Rate", value = $"{summary.AverageErrorRate}%", inline = true },
                    
                    // Performance Metrics
                    new { name = "⚡ Avg. Throughput", value = $"{summary.AverageThroughput} req/sec", inline = true },
                    new { name = "⏱️ Avg. Round Duration", value = $"{summary.AverageRoundDuration} sec", inline = true },
                    
                    // Time Metrics
                    new { name = "⏳ Timing Metrics", value = GetTimingMetricsText(summary), inline = false },
                    
                    // Resource Usage
                    new { name = "💻 Resource Usage", value = GetResourceUsageText(summary), inline = false },
                },
                timestamp = DateTime.UtcNow.ToString("o"),
                footer = new
                {
                    text = "Endurance Testing Tool by Rahma Bintang Pratama • " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                }
            };

            var embedsList = new List<object> { embed };

            if (!string.IsNullOrEmpty(aiAnalysisResult))
            {
                var aiEmbed = new
                {
                    title = "🧠 AI Analysis",
                    description = TrimAIAnalysis(aiAnalysisResult),
                    color = 0x9B59B6
                };

                embedsList.Add(aiEmbed);
            }

            return new
            {
                content = "📊 **Endurance Testing Results** 📊",
                embeds = embedsList.ToArray()
            };
        }

        private string GetParametersText(TestParameters parameters)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"**Mode**: {parameters.Mode}");
            sb.AppendLine($"**URL**: {parameters.Url}");

            if (parameters.Mode != "Stable")
            {
                sb.AppendLine($"**Min Round Requests**: {parameters.MinRequests}");
                sb.AppendLine($"**Max Round Requests**: {parameters.MaxRequests}");
            }
            else
            {
                sb.AppendLine($"**Round Requests**: {parameters.MinRequests}");
            }

            sb.AppendLine($"**Duration**: {parameters.DurationInSeconds} seconds");
            sb.AppendLine($"**Timeout**: {parameters.TimeoutInSeconds} seconds");

            sb.AppendLine($"**Time Period**: {parameters.SelectedTimePeriod}");
            return sb.ToString();
        }

        private string GetTimingMetricsText(TestSummary summary)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"**Avg. Response Time**: {summary.AverageResponseTime} ms");
            sb.AppendLine($"**Avg. Wait Time**: {summary.AverageWaitTime} ms");
            sb.AppendLine($"**Avg. Load Time**: {summary.AverageLoadTime} ms");
            sb.AppendLine($"**Total Rounds**: {summary.CurrentRound}");
            return sb.ToString();
        }

        private string GetResourceUsageText(TestSummary summary)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"**Avg. Computer's CPU Usage**: {summary.AverageCpuUsage}%");
            sb.AppendLine($"**Avg. Computer's RAM Usage**: {summary.AverageRamUsage} MB");
            return sb.ToString();
        }

        private string TrimAIAnalysis(string analysis)
        {
            if (string.IsNullOrEmpty(analysis))
                return "No AI analysis available.";

            if (analysis.Length <= 1800)
                return analysis;

            return analysis.Substring(0, 1800) + "...\n*(AI analysis truncated due to length)*";
        }
    }
}