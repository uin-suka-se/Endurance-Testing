using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Endurance_Testing.Models;

namespace Endurance_Testing.Services
{
    public class AIAnalysisService
    {
        public static async Task<string> GetAIAnalysis(string apiKey, string url, double averageCpuUsage,
            double averageRamUsage, double averageLoadTime, double averageWaitTime,
            double averageResponseTime, double averageThroughput, double averageErrorRate,
            int totalSuccessful, int totalFailed, int totalRequests)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                return "";
            }

            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    string geminiUrl = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={apiKey}";

                    string testData = $"Endurance Testing Data:\n\n" +
                                      $"URL Target: {url}\n" +
                                      $"Total Requests: {totalRequests}\n" +
                                      $"Successful Requests: {totalSuccessful}\n" +
                                      $"Failed Requests: {totalFailed}\n" +
                                      $"Average Load Time: {averageLoadTime} ms\n" +
                                      $"Average Wait Time: {averageWaitTime} ms\n" +
                                      $"Average Response Time: {averageResponseTime} ms\n" +
                                      $"Average Throughput: {averageThroughput} requests/second\n" +
                                      $"Average Error Rate: {averageErrorRate}%\n";

                    string prompt = "The following definitions apply to this analysis: " +
                                    "Response time: refers to the time spent between sending a request to the server and receiving the response. It is measured in kilobytes per second. " +
                                    "Throughput: refers to the number of requests/transactions processed in a certain amount of time during the test. It shows the amount of the required capacity that the Application Under Test can handle. Throughput depends on the number of concurrent users. " +
                                    "Wait time: It is called the average latency. It refers to the time taken until the developer receives the first byte after sending a request. " +
                                    "Average load time: refers to the average amount of time taken to receive each request. It reflects the quality and the responsivity of the Application Under Test from the user’s perspective. " +
                                    "Error rate: refers to the ratio between the failed requests and all requests.The ratio is calculated in percentage.The failed requests always occur when the load exceeds the capacity of the Application Under Test. " +
                                    "Based on the following endurance testing data, please provide an analysis of the Application Under Test performance and identify potential issues evident from the data. " +
                                    "Please structure your response as a Performance Analysis (in 2-3 paragraphs) followed by Potential Issues. " +
                                    "Provide your answer without text formatting and use spaces rather than line breaks as separators. Ensure there is a space after each period. " +
                                    "Here is the endurance testing data: ";

                    var requestBody = new GeminiRequest
                    {
                        contents = new List<GeminiContent>
                        {
                            new GeminiContent
                            {
                                role = "user",
                                parts = new List<GeminiPart>
                                {
                                    new GeminiPart { text = prompt + testData }
                                }
                            }
                        }
                    };

                    string requestJson = System.Text.Json.JsonSerializer.Serialize(requestBody);

                    StringContent content = new StringContent(requestJson, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await httpClient.PostAsync(geminiUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        var geminiResponse = System.Text.Json.JsonSerializer.Deserialize<GeminiResponse>(responseBody);

                        if (geminiResponse?.candidates != null && geminiResponse.candidates.Count > 0)
                        {
                            return geminiResponse.candidates[0].content.parts[0].text;
                        }
                        else
                        {
                            return "No analysis generated by AI.";
                        }
                    }
                    else
                    {
                        string errorResponse = await response.Content.ReadAsStringAsync();
                        return $"Error accessing Gemini API: {response.StatusCode}. Details: {errorResponse}";
                    }
                }
            }
            catch (Exception ex)
            {
                return $"An error occured: {ex.Message}";
            }
        }
    }
}