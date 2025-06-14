﻿# Endurance Testing

<h1 align="center">
    <img src="ic_endurance_testing.png" alt="App Icon" width="300"/>
</h1>

## Description

The Endurance Testing Software is a tool for evaluating the performance and stability of web applications through continuous load testing. It enables users to send multiple HTTP requests simultaneously to a specified URL while monitoring various performance metrics such as load time, wait time, response time, throughput, and error rate.

## Features

- **Three Testing Modes**: 
  - Stable: Consistent number of requests per round
  - Progressive: Gradually increasing number of requests over time
  - Fluctuative: Random variation between minimum and maximum requests

- **Comprehensive Metrics**: 
  - Computer's CPU and RAM monitoring
  - Load, wait, and response times
  - Throughput and error rates
  - Round duration

- **Export Capabilities**: 
  - Excel (.xlsx)
  - CSV (.csv)
  - JSON (.json)
  - HTML (.html)

- **Integrations**: 
  - AI analysis with Gemini API
  - Results delivery to Discord via webhook

- **Visualization and Analytics**:
  - Modern interface design
  - Test summary analysis

## Installation

1. Download the latest release from the [Releases page](https://github.com/uin-suka-se/Endurance-Testing/releases)
2. Extract the ZIP file to your desired location
3. Run the installer package `Endurance-Testing-Installer.msi`

### Prerequisites

- Windows 7 or newer
- .NET 8.0 Runtime
- 64-bit processor
- 16GB RAM or higher

## How to Use

### Starting a Basic Test

1. Enter the target URL to be tested
2. Specify the minimum number of requests per round (maximum 1000)
3. For Progressive/Fluctuative modes, also specify the maximum number of requests
4. Select the testing mode (Stable/Progressive/Fluctuative)
5. Set the timeout per round in seconds
6. Determine the test duration and select the time unit (seconds/minutes/hours)
7. (Optional) Enter a Gemini API key for AI analysis
8. (Optional) Enter a Discord webhook URL for automatic notifications
9. Click the "Start" button to begin testing

### Understanding the Output

After testing completes, the application will display:

1. **Request Information**:
   - Total processed requests
   - Number of successful and failed requests

2. **Performance Metrics**:
   - Average computer's CPU and RAM usage
   - Average load time, wait time, and response time
   - Average throughput (requests per second)
   - Average error rate
   - Average round duration

3. **AI Analysis** (if Gemini API key is provided):
   - Performance evaluation of the tested web application
   - Identification of potential issues

4. **Automatic Notifications**:
   - Test summary is automatically sent to Discord server when testing completes if Discord webhook URL has been provided
   - Notification includes key metrics and overall performance assessment

### Exporting Results

Click the "Export" button to save results in Excel, CSV, JSON, or HTML format, or send a test summary to a Discord server via webhook.

## Testing Metrics

### 1. Computer's CPU Usage
- **Description**: Percentage of the computer's processing power consumed during the test
- **Formula**: Computer's CPU Usage = Current Computer's CPU Utilization Percentage

### 2. Computer's RAM Usage
- **Description**: Amount of computer memory utilized during the test
- **Formula**: Computer's RAM Usage = Current Computer's RAM Utilization in Megabytes

### 3. Total Requests
- **Description**: Aggregate count of requests dispatched during the test
- **Formula**: Total Requests = Sum of Requests Dispatched

### 4. Successful Requests
- **Description**: Count of requests that receive an HTTP 200 (OK) response
- **Formula**: Successful Requests = Count of Requests with HTTP Status Code 200 (OK)

### 5. Failed Requests
- **Description**: Count of requests that fail
- **Formula**: Failed Requests = Total Requests − Successful Requests

### 6. Average Load Time
- **Description**: Average time taken to fully receive each request
- **Formula**: Average Load Time = Total Load Time / Total Requests

### 7. Average Wait Time
- **Description**: Average time until receiving the first byte after sending a request
- **Formula**: Average Wait Time = Total Wait Time / Total Requests

### 8. Average Response Time
- **Description**: Mean response time per request during the test
- **Formula**: Average Response Time = Total Response Time / Total Requests

### 9. Throughput
- **Description**: Number of successful requests processed per second
- **Formula**: Throughput = Successful Requests / Total Test Duration (in seconds)

### 10. Error Rate
- **Description**: Percentage of requests that failed during the test
- **Formula**: Error Rate = (Failed Requests / Total Requests) * 100

### 11. Round Duration
- **Description**: Actual duration for each testing round, capped by the timeout value
- **Formula**: Round Duration = Round Time, but if it exceeds Timeout Duration then Round Time = Timeout Duration

## User Guide

1. Enter the target URL you wish to test in the text field labeled 'URL:' (must use http:// or https://)
2. Enter the minimum number of requests to dispatch per round (maximum 1000 requests)
3. For 'Progressive' or 'Fluctuative' modes, also enter the maximum number of requests
4. Select the desired test mode from the dropdown menu:
   - Stable: Dispatches a consistent number of requests in each test round
   - Progressive: Gradually increases the number of requests per round over time
   - Fluctuative: Dispatches a random number of requests within the defined range
5. Enter the timeout threshold in seconds for each test round
6. Enter the test duration and select the time unit (seconds/minutes/hours)
7. (Optional) Enter your Gemini API key for AI-powered descriptive test summary
8. (Optional) Enter Discord Webhook URL for automatic summary delivery
9. Click the 'Start' button to initiate the endurance test
10. Click the 'Stop' button if you want to stop the test before the countdown finishes
11. Monitor the test results in the output text box and remaining time above the output text box
12. After testing completes, use the 'Export' button to save results

> **Note**: Ensure that your internet connection is stable and reliable for conducting this test. Device performance may be reduced during the testing process. The output text box only displays the latest round results - use the export feature to save all data or view EnduranceTestLog.txt in the same directory with your executable file.

## License

This **Endurance Testing** project is licensed under the [MIT License](LICENSE).

## Third-Party Libraries

This project uses the following third-party libraries, each with their own licenses:

- CircularProgressBar_NET5 3.0.0
- ClosedXML 0.104.2
- Microsoft.Extensions.DependencyInjection 9.03
- Microsoft.Extensions.Http 9.03
- Microsoft.Web.WebView2 1.0.3124.44

For more information, refer to the respective license files provided with these libraries.

## Contact and Information

This application was developed by Rahma Bintang Pratama.

Project Link: [https://github.com/uin-suka-se/Endurance-Testing](https://github.com/uin-suka-se/Endurance-Testing)

> "Endurance testing, also known as soak testing, involves subjecting an application to a sustained load for an extended period. This methodology helps uncover memory leaks, resource depletion, and other performance degradation issues that might only surface after prolonged usage." [`[1]`](https://www.doi.org/10.21275/SR23822111402)
> 
> [`[1]`](https://www.doi.org/10.21275/SR23822111402) S. Pargaonkar, "A Comprehensive Review of Performance Testing Methodologies and Best Practices: Software Quality Engineering," _International Journal of Science and Research (IJSR)_, vol. 12, no. 8, pp. 2008-2014, August 2023.
