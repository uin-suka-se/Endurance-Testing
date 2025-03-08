using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Endurance_Testing.Services
{
    public class SystemMonitor
    {
        public static async Task<double> GetCpuUsage()
        {
            return await Task.Run(() =>
            {
                using (var cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total"))
                {
                    cpuCounter.NextValue();
                    Thread.Sleep(1000);
                    return cpuCounter.NextValue();
                }
            });
        }

        public static async Task<double> GetRamUsage()
        {
            return await Task.Run(() =>
            {
                using (Process currentProcess = Process.GetCurrentProcess())
                {
                    return currentProcess.WorkingSet64 / (1024 * 1024.0);
                }
            });
        }
    }
}