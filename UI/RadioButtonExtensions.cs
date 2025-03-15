using System;
using System.Drawing;
using System.Windows.Forms;

namespace Endurance_Testing.UI
{
    public static class RadioButtonExtensions
    {
        public static void MakeBold(this RadioButton radioButton,
                                    Color foreColor,
                                    Color activeForeColor,
                                    int borderWidth = 2)
        {
            Color disabledForeColor = Color.FromArgb(90, 90, 90);

            radioButton.Tag = new object[] { foreColor, disabledForeColor, activeForeColor };

            UpdateRadioButtonAppearance(radioButton);

            radioButton.FlatStyle = FlatStyle.Flat;
            radioButton.FlatAppearance.BorderSize = borderWidth;

            radioButton.CheckedChanged -= RadioButton_CheckedChanged;
            radioButton.EnabledChanged -= RadioButton_EnabledChanged;

            radioButton.CheckedChanged += RadioButton_CheckedChanged;
            radioButton.EnabledChanged += RadioButton_EnabledChanged;
        }

        private static void RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;
            UpdateRadioButtonAppearance(radioButton);
        }

        private static void RadioButton_EnabledChanged(object sender, EventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;
            UpdateRadioButtonAppearance(radioButton);
        }

        private static void UpdateRadioButtonAppearance(RadioButton radioButton)
        {
            if (!(radioButton.Tag is object[] colors) || colors.Length < 3)
                return;

            if (radioButton.Enabled)
            {
                radioButton.ForeColor = radioButton.Checked ? (Color)colors[2] : (Color)colors[0];
                radioButton.Font = new Font(radioButton.Font, radioButton.Checked ? FontStyle.Bold : FontStyle.Regular);
            }
            else
            {
                radioButton.ForeColor = (Color)colors[1];
                radioButton.Font = new Font(radioButton.Font, FontStyle.Regular);
            }

            radioButton.Invalidate();
        }
    }
}