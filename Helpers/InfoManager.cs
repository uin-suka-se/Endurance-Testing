using System.Text;

namespace Endurance_Testing.Helpers
{
    public static class InfoManager
    {
        public static string GenerateInfoMessage()
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
            infoMessage.AppendLine("6. Average Load Time:");
            infoMessage.AppendLine("    a. Description:");
            infoMessage.AppendLine("        Average Load Time refers to the average amount of time taken to fully receive each request, reflecting the quality and responsivity of the application from the user's perspective.");
            infoMessage.AppendLine("    b. Formula:");
            infoMessage.AppendLine("        Average Load Time = Total Load Time / Total Requests");
            infoMessage.AppendLine("7. Average Wait Time:");
            infoMessage.AppendLine("    a. Description:");
            infoMessage.AppendLine("        Average Wait Time refers to the average time taken until receiving the first byte after sending a request.");
            infoMessage.AppendLine("    b. Formula:");
            infoMessage.AppendLine("        Average Wait Time = Total Wait Time / Total Requests");
            infoMessage.AppendLine("8. Average Response Time:");
            infoMessage.AppendLine("    a. Description:");
            infoMessage.AppendLine("        Average Response Time represents the mean response time per request during the endurance test (including successful and failed requests).");
            infoMessage.AppendLine("    b. Formula:");
            infoMessage.AppendLine("        Average Response Time = Total Response Time / Total Requests");
            infoMessage.AppendLine("9. Throughput:");
            infoMessage.AppendLine("    a. Description:");
            infoMessage.AppendLine("        Throughput measures the number of successful requests processed per second during the endurance test.");
            infoMessage.AppendLine("    b. Formula:");
            infoMessage.AppendLine("        Throughput = Successful Requests / Total Test Duration (in seconds)");
            infoMessage.AppendLine("10. Error Rate:");
            infoMessage.AppendLine("    a. Description:");
            infoMessage.AppendLine("        Error Rate indicates the percentage of requests that failed during the endurance test.");
            infoMessage.AppendLine("    b. Formula:");
            infoMessage.AppendLine("        Error Rate = (Failed Requests / Total Requests) * 100");
            infoMessage.AppendLine("11. Round Duration:");
            infoMessage.AppendLine("    a. Description:");
            infoMessage.AppendLine("        Round duration indicates the actual duration for each testing round, capped by the timeout value.");
            infoMessage.AppendLine("    b. Formula:");
            infoMessage.AppendLine("        Round Duration = Round Time, but if it exceeds Timeout Duration then Round Time = Timeout Duration");

            return infoMessage.ToString();
        }
    }
}