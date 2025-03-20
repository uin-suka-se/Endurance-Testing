using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Endurance_Testing.UI
{
    public partial class UserGuideEN : MacStyleTitleBar
    {
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]

        private static extern IntPtr CreateRoundRectRgn
        (
            int nLeftRect,
            int nTopRect,
            int nRightRect,
            int nBottomRect,
            int nWidthEllipse,
            int nHeightEllipse
        );

        public UserGuideEN()
        {
            InitializeComponent();

            Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));

            InitializeWebView2();
            this.Load += new EventHandler(this.UserGuideEN_Load);
        }

        private async void InitializeWebView2()
        {
            await webViewUserGuideEN.EnsureCoreWebView2Async(null);
        }

        private void UserGuideEN_Load(object sender, EventArgs e)
        {
            string pdfPath = Path.Combine(Application.StartupPath, "UserGuideEN.pdf");

            if (File.Exists(pdfPath))
            {
                webViewUserGuideEN.Source = new Uri(pdfPath);
            }
            else
            {
                MessageBox.Show("User guide file not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
