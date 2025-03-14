using System;
using System.IO;
using System.Text;

namespace Endurance_Testing.Services
{
    public static class LogService
    {
        private static readonly string logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "EnduranceTestLog.txt");

        public static void InitializeLog()
        {
            if (File.Exists(logFilePath))
            {
                File.Delete(logFilePath);
            }
        }

        public static void WriteLog(string message)
        {
            try
            {
                using (FileStream fs = new FileStream(logFilePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                {
                    using (StreamWriter writer = new StreamWriter(fs, Encoding.UTF8))
                    {
                        writer.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss}\n{message}");
                        writer.Flush();
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exception
            }
        }
    }
}