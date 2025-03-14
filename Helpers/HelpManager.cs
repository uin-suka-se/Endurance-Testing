using System.Text;

namespace Endurance_Testing.Helpers
{
    public static class HelpManager
    {
        public static string GenerateHelpMessage()
        {
            StringBuilder helpMessage = new StringBuilder();

            helpMessage.AppendLine("User Guide for Endurance Testing Tool:");
            helpMessage.AppendLine();
            helpMessage.AppendLine("1. Enter the target URL you wish to test in the text field labeled 'URL:'.");
            helpMessage.AppendLine("2. Enter the minimum number of requests to dispatch per round in the text field labeled 'Number of Request (Min and Max)' (maximum 8 digits).");
            helpMessage.AppendLine("   - For 'Progressive' or 'Fluctuative' modes, also enter the maximum number of requests to dispatch per round in the same field.");
            helpMessage.AppendLine("3. (Optional) Enter your Gemini API key in the settings to enable AI-powered test result analysis.");
            helpMessage.AppendLine("4. Select the desired test mode from the dropdown menu labeled 'Mode:' (Stable, Progressive, or Fluctuative).");
            helpMessage.AppendLine("   - Stable: Dispatches a consistent number of requests in each test round.");
            helpMessage.AppendLine("   - Progressive: Gradually increases the number of requests per round over the duration of the test.");
            helpMessage.AppendLine("   - Fluctuative: Dispatches a random number of requests within the defined minimum and maximum range for each round.");
            helpMessage.AppendLine("5. Enter the timeout threshold in seconds for each test round in the text field labeled 'Timeout Per-Round (In Seconds):'.");
            helpMessage.AppendLine("6. Enter the test duration in the 'Time in Period:' field and select the unit of time (seconds, minutes, or hours) using the radio buttons to the right of this field.");
            helpMessage.AppendLine("7. (Optional) Enter Discord Webhook URL in the text field labeled 'Discord Webhook URL:' to automatically send test summary to Discord Server after completion.");
            helpMessage.AppendLine("8. Click the 'Start' button to initiate the endurance test.");
            helpMessage.AppendLine("9. Monitor the test results in the 'Output:' text area below the input fields and the remaining time above the output area.");
            helpMessage.AppendLine("10. Upon test completion, the 'Output:' area will display:");
            helpMessage.AppendLine("    - Total Requests: The total number of requests sent during the test.");
            helpMessage.AppendLine("    - Successful Requests: The number of requests that received a successful HTTP 200 (OK) response.");
            helpMessage.AppendLine("    - Failed Requests: The number of requests that did not receive an HTTP 200 (OK) response or timed out.");
            helpMessage.AppendLine("    - Average Computer's CPU Usage: The average percentage of computer's CPU utilization during the test.");
            helpMessage.AppendLine("    - Average Computer's RAM Usage: The average computer's RAM utilization in megabytes during the test.");
            helpMessage.AppendLine("    - Average Load Time: The average amount of time taken to fully receive each request, reflecting the quality and responsiveness from the user's perspective.");
            helpMessage.AppendLine("    - Average Wait Time: The average time taken until receiving the first byte after sending a request.");
            helpMessage.AppendLine("    - Average Response Time: The average response time for all requests (including successful and failed).");
            helpMessage.AppendLine("    - Average Throughput: The average number of requests processed per second.");
            helpMessage.AppendLine("    - Average Error Rate: The percentage of requests that failed or timed out.");
            helpMessage.AppendLine("    - Average Round Duration: The average time in seconds it takes to complete one round of requests.");
            helpMessage.AppendLine("    - AI Analysis: If Gemini API key is provided, an AI-generated analysis of test results.");
            helpMessage.AppendLine("11. Click the 'Clear' button to reset the input fields and the output area.");
            helpMessage.AppendLine("12. Click the 'Export' button to save test results in various formats:");
            helpMessage.AppendLine("    - Excel (.xlsx)");
            helpMessage.AppendLine("    - CSV (.csv)");
            helpMessage.AppendLine("    - JSON (.json)");
            helpMessage.AppendLine("    - HTML (.html)");
            helpMessage.AppendLine("    - Discord (Test summary only)");
            helpMessage.AppendLine();
            helpMessage.AppendLine("Note:");
            helpMessage.AppendLine("   - Ensure that your internet connection is stable and reliable for conducting this test.");
            helpMessage.AppendLine("   - Be aware that device performance may be reduced during the testing process and confirm that your device specifications are adequate.");
            helpMessage.AppendLine("   - The actual test duration may vary slightly from the input time due to the processing time for handling requests and responses.");
            helpMessage.AppendLine("   - The Gemini API key is required to enable AI analysis of test results. Without a key, the application will still function but without AI features.");

            return helpMessage.ToString();
        }
    }
}