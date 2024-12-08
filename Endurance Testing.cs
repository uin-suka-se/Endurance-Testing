using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Endurance_Testing
{
    public partial class EnduranceTesting : Form
    {
        private CancellationTokenSource cancellationTokenSource;
        private List<EnduranceTestResult> enduranceTestResults = new List<EnduranceTestResult>();
        private int totalRequests;
        private int durationInSeconds;
        private int currentRound;
        private int totalSuccessfulRequests; // Total permintaan berhasil untuk semua putaran
        private int totalFailedRequests; // Total permintaan gagal untuk semua putaran
        private float totalCpuUsage;
        private float totalRamUsage;

        public EnduranceTesting()
        {
            InitializeComponent();
            this.Load += EnduranceTesting_Load;
            textBoxInputRequest.KeyPress += new KeyPressEventHandler(textBoxOnlyNumber_KeyPress);
            textBoxTime.KeyPress += new KeyPressEventHandler(textBoxOnlyNumber_KeyPress);

            btnStop.Enabled = false;
        }

        private void EnduranceTesting_Load(object sender, EventArgs e)
        {
            // Inisialisasi jika diperlukan
        }

        private void textBoxOnlyNumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Hanya izinkan input angka dan kontrol khusus (seperti Backspace)
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private async void btnStart_Click(object sender, EventArgs e)
        {
            // Validasi input URL
            string url = textBoxInputUrl.Text;
            if (string.IsNullOrWhiteSpace(url))
            {
                MessageBox.Show("Please enter a valid URL.");
                return;
            }

            // Validasi input request
            if (!int.TryParse(textBoxInputRequest.Text, out totalRequests) || totalRequests <= 0)
            {
                MessageBox.Show("Please enter a valid number of requests.");
                return;
            }

            // Validasi input waktu
            if (!int.TryParse(textBoxTime.Text, out durationInSeconds) || durationInSeconds <= 0)
            {
                MessageBox.Show("Please enter a valid duration.");
                return;
            }

            // Menentukan durasi dalam detik berdasarkan pilihan periode
            if (radioButtonMinute.Checked)
            {
                durationInSeconds *= 60;
            }
            else if (radioButtonHour.Checked)
            {
                durationInSeconds *= 3600;
            }
            else if (!radioButtonSecond.Checked) // Pastikan salah satu periode dipilih
            {
                MessageBox.Show("Please select a time period (seconds, minutes, or hours).");
                return;
            }

            // Nonaktifkan tombol Start, Stop, Clear, dan Export
            btnStart.Enabled = false;
            btnStop.Enabled = true;
            btnClear.Enabled = false;
            btnExport.Enabled = false;

            cancellationTokenSource = new CancellationTokenSource();
            currentRound = 0; // Reset nomor putaran
            totalSuccessfulRequests = 0; // Reset total permintaan berhasil
            totalFailedRequests = 0; // Reset total permintaan gagal
            totalCpuUsage = 0; // Reset total penggunaan CPU
            totalRamUsage = 0; // Reset total penggunaan RAM

            // Mulai penghitungan mundur waktu
            var countdownTask = StartCountdown(durationInSeconds, cancellationTokenSource.Token);

            // Mulai pengujian
            await RunEnduranceTest(url, cancellationTokenSource.Token);

            // Tunggu hingga penghitungan mundur selesai
            await countdownTask;

            // Aktifkan kembali tombol Clear dan Export setelah pengujian selesai
            btnClear.Enabled = true;
            btnExport.Enabled = true;

            // Tampilkan ringkasan setelah pengujian selesai di output
            ShowSummary();
        }

        private async Task StartCountdown(int durationInSeconds, CancellationToken cancellationToken)
        {
            for (int i = durationInSeconds; i > 0; i--)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return; // Hentikan penghitungan mundur jika diminta
                }

                TimeSpan timeLeft = TimeSpan.FromSeconds(i);
                lblTimeLeft.Text = $"{timeLeft:hh\\:mm\\:ss}"; // Hanya angka dalam format HH:mm:ss
                await Task.Delay(1000); // Tunggu 1 detik
            }

            // Setelah loop selesai, setel label ke 00:00:00
            lblTimeLeft.Text = "00:00:00";

            // Jika waktu habis, cancel pengujian
            cancellationTokenSource.Cancel();
        }

        private async Task RunEnduranceTest(string url, CancellationToken cancellationToken)
        {
            HttpClient httpClient = new HttpClient();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            while (!cancellationToken.IsCancellationRequested)
            {
                currentRound++; // Increment nomor putaran

                // Reset permintaan berhasil dan gagal untuk putaran ini
                int roundSuccessfulRequests = 0;
                int roundFailedRequests = 0;

                // Kirim permintaan secara bersamaan
                var tasks = new List<Task<EnduranceTestResult>>();
                for (int i = 0; i < totalRequests; i++)
                {
                    tasks.Add(SendHttpRequest(httpClient, url));
                }

                // Tunggu hingga semua permintaan selesai
                var results = await Task.WhenAll(tasks);
                enduranceTestResults.AddRange(results);

                // Hitung penggunaan CPU dan RAM
                float currentCpuUsage = GetCpuUsage();
                float currentRamUsage = GetRamUsage();
                totalCpuUsage += currentCpuUsage;
                totalRamUsage += currentRamUsage;

                // Tampilkan hasil secara real-time dengan informasi putaran
                foreach (var result in results)
                {
                    DisplayResult(result, currentRound);
                    if (result.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        roundSuccessfulRequests++;
                        totalSuccessfulRequests++;
                    }
                    else
                    {
                        roundFailedRequests++;
                        totalFailedRequests++;
                    }
                }

                // Tampilkan statistik untuk putaran saat ini
                DisplayRoundStatistics(currentCpuUsage, currentRamUsage, roundSuccessfulRequests, roundFailedRequests);

                // Tunggu sebelum mengirim permintaan berikutnya
                await Task.Delay(1000); // Delay 1 detik antara putaran
            }

            stopwatch.Stop();
            btnStart.Enabled = true;
            btnStop.Enabled = false;
        }

        private async Task<EnduranceTestResult> SendHttpRequest(HttpClient httpClient, string url)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(url);
                TimeSpan responseTime = stopwatch.Elapsed;

                return new EnduranceTestResult
                {
                    StatusCode = response.StatusCode,
                    ReasonPhrase = response.ReasonPhrase,
                    ResponseTime = responseTime
                };
            }
            catch (Exception ex)
            {
                return new EnduranceTestResult
                {
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    ReasonPhrase = ex.Message,
                    ResponseTime = stopwatch.Elapsed
                };
            }
            finally
            {
                stopwatch.Stop();
            }
        }

        private void DisplayResult(EnduranceTestResult result, int round)
        {
            string resultString = $"Round {round}: Status: {result.StatusCode}, Reason: {result.ReasonPhrase}, Response Time: {result.ResponseTime.TotalMilliseconds} ms";
            textBoxOutput.AppendText(resultString + Environment.NewLine);
            textBoxOutput.ScrollToCaret(); // Scroll ke bawah untuk menampilkan hasil terbaru
        }

        private void DisplayRoundStatistics(float currentCpuUsage, float currentRamUsage, int roundSuccessfulRequests, int roundFailedRequests)
        {
            // Tampilkan statistik untuk putaran saat ini
            string roundStats = $"Round {currentRound} Statistics:{Environment.NewLine}" +
                                $"CPU Usage: {currentCpuUsage:F2}%{Environment.NewLine}" +
                                $"RAM Usage: {currentRamUsage:F2} MB{Environment.NewLine}" +
                                $"Total Requests: {totalRequests}{Environment.NewLine}" +
                                $"Successful Requests: {roundSuccessfulRequests}{Environment.NewLine}" +
                                $"Failed Requests: {roundFailedRequests}{Environment.NewLine}{Environment.NewLine}"; // Tambahkan baris baru

            textBoxOutput.AppendText(roundStats);
            textBoxOutput.ScrollToCaret(); // Scroll ke bawah untuk menampilkan statistik terbaru
        }

        private void ShowSummary()
        {
            // Hitung rata-rata penggunaan CPU dan RAM
            float averageCpuUsage = totalCpuUsage / currentRound;
            float averageRamUsage = totalRamUsage / currentRound;

            // Tampilkan ringkasan di output
            string summaryMessage = $"Summary:{Environment.NewLine}" +
                                    $"Total Requests: {totalRequests * currentRound}{Environment.NewLine}" +
                                    $"Successful Requests: {totalSuccessfulRequests}{Environment.NewLine}" +
                                    $"Failed Requests: {totalFailedRequests}{Environment.NewLine}" +
                                    $"Average CPU Usage: {averageCpuUsage:F2}%{Environment.NewLine}" +
                                    $"Average RAM Usage: {averageRamUsage:F2} MB{Environment.NewLine}"; // Tambahkan baris baru

            textBoxOutput.AppendText(summaryMessage);
            textBoxOutput.ScrollToCaret(); // Scroll ke bawah untuk menampilkan ringkasan terbaru
        }

        private float GetCpuUsage()
        {
            // Menggunakan PerformanceCounter untuk mendapatkan penggunaan CPU
            using (var cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total"))
            {
                return cpuCounter.NextValue();
            }
        }

        private float GetRamUsage()
        {
            // Menggunakan PerformanceCounter untuk mendapatkan penggunaan RAM
            using (var ramCounter = new PerformanceCounter("Memory", "Available MBytes"))
            {
                return ramCounter.NextValue();
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            cancellationTokenSource.Cancel();
            btnStart.Enabled = true;
            btnStop.Enabled = false;
            btnClear.Enabled = true; // Aktifkan tombol Clear setelah dihentikan
            btnExport.Enabled = true; // Aktifkan tombol Export setelah dihentikan
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            ShowHelpMessageBox();
        }

        private void ShowHelpMessageBox()
        {
            string helpMessage = GenerateHelpMessage();
            MessageBox.Show(helpMessage, "User Guide", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private string GenerateHelpMessage()
        {
            return "User Guide for Endurance Testing:\n" +
                   "1. Enter the target URL in the URL column.\n" +
                   "2. Enter the number of requests in the Requests column.\n" +
                   "3. Enter the duration for the endurance test in the Time column.\n" +
                   "4. Select the time period (seconds, minutes, hours).\n" +
                   "5. Click the 'Start' button to begin the endurance testing.\n" +
                   "6. Monitor the results in the output area.\n" +
                   "7. Click 'Stop' to halt the testing process.\n" +
                   "8. Use 'Clear' to reset the input fields and output.";
        }

        private void btnInfo_Click(object sender, EventArgs e)
        {
            ShowInfoMessageBox();
        }

        private void ShowInfoMessageBox()
        {
            string infoMessage = "Endurance Testing:\n" +
                                 "Endurance testing is a type of performance testing that determines how a system behaves under sustained load. It helps identify performance issues and ensures the system can handle prolonged usage.";
            MessageBox.Show(infoMessage, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            textBoxInputUrl.Clear();
            textBoxInputRequest.Clear();
            textBoxTime.Clear();
            lblTimeLeft.Text = "00:00:00"; // Reset label ke format default
            textBoxOutput.Clear();
            enduranceTestResults.Clear();
            totalRequests = 0;
            durationInSeconds = 0;
            btnStart.Enabled = true;
            btnStop.Enabled = false;
            btnClear.Enabled = false; // Nonaktifkan tombol Clear saat pengujian sedang berjalan
            btnExport.Enabled = false; // Nonaktifkan tombol Export saat pengujian sedang berjalan
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            // Implementasi ekspor hasil ke file CSV atau Excel
            // Anda dapat menggunakan library seperti EPPlus atau CsvHelper untuk ekspor
            MessageBox.Show("Export functionality is not implemented yet.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    public class EnduranceTestResult
    {
        public System.Net.HttpStatusCode StatusCode { get; set; }
        public string ReasonPhrase { get; set; }
        public TimeSpan ResponseTime { get; set; }
    }
}
