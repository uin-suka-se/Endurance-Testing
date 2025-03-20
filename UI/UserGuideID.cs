using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Endurance_Testing.UI
{
    public partial class UserGuideID : MacStyleTitleBar
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

        public UserGuideID()
        {
            InitializeComponent();

            Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));

            InitializeWebView2();
            this.Load += new EventHandler(this.UserGuideID_Load);
        }

        private async void InitializeWebView2()
        {
            await webViewUserGuideID.EnsureCoreWebView2Async(null);
        }

        private void UserGuideID_Load(object sender, EventArgs e)
        {
            string pdfPath = Path.Combine(Application.StartupPath, "UserGuideID.pdf");

            if (File.Exists(pdfPath))
            {
                webViewUserGuideID.Source = new Uri(pdfPath);
            }
            else
            {
                MessageBox.Show("User guide file not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}