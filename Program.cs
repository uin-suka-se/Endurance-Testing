using System;
using System.Windows.Forms;

using Endurance_Testing.UI;

namespace Endurance_Testing
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using (SplashScreen splash = new SplashScreen())
            {
                splash.ShowDialog();
            }

            Application.Run(new EnduranceTesting());
        }
    }
}
