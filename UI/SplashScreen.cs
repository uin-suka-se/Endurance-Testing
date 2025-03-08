using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Endurance_Testing.UI
{
    public partial class SplashScreen : Form
    {
        private int loadingDots = 0;

        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]

        private static extern IntPtr CreateRoundRectRgn(
            int nLeftRect,
            int nTopRect,
            int nRightRect,
            int nBottomRect,
            int nWidthEllipse,
            int nHeightEllipse
        );

        public SplashScreen()
        {
            InitializeComponent();
            Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 25, 25));
            circularProgressBar.Value = 0;
        }

        private void timerSplashScreen_Tick(object sender, EventArgs e)
        {
            circularProgressBar.Value += 1;
            circularProgressBar.Text = circularProgressBar.Value.ToString() + "%";

            if (circularProgressBar.Value % 5 == 0)
            {
                loadingDots = (loadingDots + 1) % 3;
                lblLoading.Text = "Loading" + new string('.', loadingDots + 1);
            }

            if (circularProgressBar.Value == 100)
            {
                timerSplashScreen.Stop();
                this.Close();
            }
        }
    }
}
