using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Endurance_Testing.UI
{
    public static class ButtonExtensions
    {
        public static void MakeRounded(this Button button,
                                     Color backColor,
                                     Color foreColor,
                                     Color hoverBackColor,
                                     Color hoverForeColor,
                                     int radius = 10)
        {
            Color disabledBackColor = FadeColor(backColor, 0.7f);
            Color disabledForeColor = Color.FromArgb(90, 90, 90);

            button.Tag = new object[] { backColor, foreColor, hoverBackColor, hoverForeColor, disabledBackColor, disabledForeColor };

            UpdateButtonAppearance(button);

            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;

            GraphicsPath path = new GraphicsPath();
            path.AddArc(0, 0, radius, radius, 180, 90);
            path.AddArc(button.Width - radius, 0, radius, radius, 270, 90);
            path.AddArc(button.Width - radius, button.Height - radius, radius, radius, 0, 90);
            path.AddArc(0, button.Height - radius, radius, radius, 90, 90);
            path.CloseFigure();
            button.Region = new Region(path);

            button.MouseEnter -= Button_MouseEnter;
            button.MouseLeave -= Button_MouseLeave;
            button.Resize -= Button_Resize;
            button.EnabledChanged -= Button_EnabledChanged;

            button.MouseEnter += Button_MouseEnter;
            button.MouseLeave += Button_MouseLeave;
            button.Resize += Button_Resize;
            button.EnabledChanged += Button_EnabledChanged;
        }

        private static void Button_MouseEnter(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            if (!button.Enabled) return;

            if (button.Tag is object[] colors && colors.Length >= 4)
            {
                // Ganti warna saat hover
                button.BackColor = (Color)colors[2];
                button.ForeColor = (Color)colors[3];
                button.Cursor = Cursors.Hand;
            }
        }

        private static void Button_MouseLeave(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            if (!button.Enabled) return;

            if (button.Tag is object[] colors && colors.Length >= 2)
            {
                button.BackColor = (Color)colors[0];
                button.ForeColor = (Color)colors[1];
            }
        }

        private static void Button_Resize(object sender, EventArgs e)
        {
            Button button = (Button)sender;

            int radius = 10;
            if (button.Region != null)
            {
                button.Region.Dispose();
            }

            GraphicsPath path = new GraphicsPath();
            path.AddArc(0, 0, radius, radius, 180, 90);
            path.AddArc(button.Width - radius, 0, radius, radius, 270, 90);
            path.AddArc(button.Width - radius, button.Height - radius, radius, radius, 0, 90);
            path.AddArc(0, button.Height - radius, radius, radius, 90, 90);
            path.CloseFigure();
            button.Region = new Region(path);
        }

        private static void Button_EnabledChanged(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            UpdateButtonAppearance(button);
        }

        private static void UpdateButtonAppearance(Button button)
        {
            if (!(button.Tag is object[] colors) || colors.Length < 6)
                return;

            if (button.Enabled)
            {
                button.BackColor = (Color)colors[0];
                button.ForeColor = (Color)colors[1];
            }
            else
            {
                button.BackColor = (Color)colors[4];
                button.ForeColor = (Color)colors[5];
            }
        }

        private static Color FadeColor(Color color, float factor)
        {
            int r = (int)((255 - color.R) * factor + color.R);
            int g = (int)((255 - color.G) * factor + color.G);
            int b = (int)((255 - color.B) * factor + color.B);
            return Color.FromArgb(color.A, r, g, b);
        }

        // For future usage (if any)
        //public static void MakeRounded(this Button button, Color backColor, Color hoverBackColor, int radius = 10)
        //{
        //    MakeRounded(button, backColor, Color.White, hoverBackColor, Color.FromArgb(230, 230, 230), radius);
        //}

        //public static void MakeRounded(this Button button)
        //{
        //    MakeRounded(
        //        button,
        //        Color.FromArgb(64, 158, 255),
        //        Color.White,
        //        Color.FromArgb(45, 111, 179),
        //        Color.FromArgb(230, 230, 230),
        //        10
        //    );
        //}
    }
}