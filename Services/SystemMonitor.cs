using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Endurance_Testing.Services
{
    public class SystemMonitor
    {
        private static Process _currentProcess = Process.GetCurrentProcess();
        private static DateTime _lastCpuCheck = DateTime.UtcNow;
        private static TimeSpan _lastCpuTime = _currentProcess.TotalProcessorTime;
        private static readonly object _lockObject = new object();

        public static async Task<double> GetCpuUsage()
        {
            return await Task.Run(() =>
            {
                lock (_lockObject)
                {
                    DateTime now = DateTime.UtcNow;
                    TimeSpan currentCpuTime = _currentProcess.TotalProcessorTime;

                    double cpuUsedMs = (currentCpuTime - _lastCpuTime).TotalMilliseconds;
                    double totalElapsedMs = (now - _lastCpuCheck).TotalMilliseconds;

                    if (totalElapsedMs == 0)
                    {
                        return 0;
                    }

                    double cpuUsagePercent = (cpuUsedMs / (Environment.ProcessorCount * totalElapsedMs)) * 100;

                    _lastCpuTime = currentCpuTime;
                    _lastCpuCheck = now;

                    return Math.Min(100, Math.Max(0, cpuUsagePercent));
                }
            });
        }

        public static async Task<double> GetRamUsage()
        {
            return await Task.Run(() =>
            {
                _currentProcess.Refresh();

                double privateBytes = _currentProcess.PrivateMemorySize64 / (1024.0 * 1024.0);

                return Math.Round(privateBytes, 2);
            });
        }
    }
}