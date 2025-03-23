using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Endurance_Testing.UI
{
    public class MacStyleTitleBar : Form
    {
        private Panel titleBar;
        private Label lblTitle;
        private Button btnClose;
        private Button btnMaximize;
        private Button btnMinimize;

        private readonly Color CLOSE_BUTTON_COLOR = Color.FromArgb(255, 95, 87);
        private readonly Color MINIMIZE_BUTTON_COLOR = Color.FromArgb(255, 189, 46);
        private readonly Color MAXIMIZE_BUTTON_COLOR = Color.FromArgb(39, 201, 63);
        private readonly int TITLE_BAR_HEIGHT = 32;
        private readonly int BUTTON_SIZE = 12;
        private readonly int BUTTON_MARGIN = 8;

        public Panel ContentPanel { get; private set; }

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MacStyleTitleBar));
            SuspendLayout();
            // 
            // MacStyleTitleBar
            // 
            ClientSize = new Size(284, 261);
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Pixel);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "MacStyleTitleBar";
            StartPosition = FormStartPosition.CenterScreen;
            ResumeLayout(false);
        }

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HT_CAPTION = 0x2;

        public MacStyleTitleBar()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.DoubleBuffered = true;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

            InitializeTitleBar();
        }

        private void InitializeTitleBar()
        {
            // Title Bar
            titleBar = new Panel
            {
                Dock = DockStyle.Top,
                Height = TITLE_BAR_HEIGHT,
                BackColor = Color.FromArgb(45, 45, 48)
            };

            // Close Button
            btnClose = CreateCircleButton(CLOSE_BUTTON_COLOR);
            btnClose.Location = new Point(BUTTON_MARGIN, (TITLE_BAR_HEIGHT - BUTTON_SIZE) / 2);
            btnClose.Click += (sender, e) => this.Close();

            // Minimize Button
            btnMinimize = CreateCircleButton(MINIMIZE_BUTTON_COLOR);
            btnMinimize.Location = new Point(btnClose.Right + BUTTON_MARGIN, (TITLE_BAR_HEIGHT - BUTTON_SIZE) / 2);
            btnMinimize.Click += (sender, e) => this.WindowState = FormWindowState.Minimized;

            // Maximize Button
            btnMaximize = CreateCircleButton(MAXIMIZE_BUTTON_COLOR);
            btnMaximize.Location = new Point(btnMinimize.Right + BUTTON_MARGIN, (TITLE_BAR_HEIGHT - BUTTON_SIZE) / 2);
            btnMaximize.Click += (sender, e) =>
            {
                if (this.WindowState == FormWindowState.Maximized)
                    this.WindowState = FormWindowState.Normal;
                else
                    this.WindowState = FormWindowState.Maximized;
            };

            lblTitle = new Label
            {
                Text = "Endurance Testing",
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.White
            };

            titleBar.Controls.Add(btnClose);
            titleBar.Controls.Add(btnMinimize);
            titleBar.Controls.Add(btnMaximize);
            titleBar.Controls.Add(lblTitle);

            titleBar.MouseDown += TitleBar_MouseDown;
            lblTitle.MouseDown += TitleBar_MouseDown;

            AddHoverEffects(btnClose, CLOSE_BUTTON_COLOR);
            AddHoverEffects(btnMinimize, MINIMIZE_BUTTON_COLOR);
            AddHoverEffects(btnMaximize, MAXIMIZE_BUTTON_COLOR);

            this.Controls.Add(titleBar);

            ContentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(10)
            };
            this.Controls.Add(ContentPanel);

            titleBar.BringToFront();
        }

        private Button CreateCircleButton(Color color)
        {
            Button button = new Button
            {
                FlatStyle = FlatStyle.Flat,
                Size = new Size(BUTTON_SIZE, BUTTON_SIZE),
                BackColor = color,
                FlatAppearance = { BorderSize = 0 }
            };

            button.Paint += (sender, e) =>
            {
                using (GraphicsPath path = new GraphicsPath())
                {
                    path.AddEllipse(0, 0, button.Width, button.Height);
                    button.Region = new Region(path);
                }
            };

            return button;
        }

        private void AddHoverEffects(Button button, Color baseColor)
        {
            button.MouseEnter += (sender, e) =>
            {
                button.BackColor = ControlPaint.Light(baseColor, 0.2f);
            };

            button.MouseLeave += (sender, e) =>
            {
                button.BackColor = baseColor;
            };
        }

        private void TitleBar_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(this.Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            ControlPaint.DrawBorder(e.Graphics, this.ClientRectangle,
                Color.FromArgb(200, 200, 200), 1, ButtonBorderStyle.Solid,
                Color.FromArgb(200, 200, 200), 1, ButtonBorderStyle.Solid,
                Color.FromArgb(200, 200, 200), 1, ButtonBorderStyle.Solid,
                Color.FromArgb(200, 200, 200), 1, ButtonBorderStyle.Solid);
        }

        public void SetTitle(string title)
        {
            this.Text = title;
            lblTitle.Text = title;
        }

        public override string Text
        {
            get { return base.Text; }
            set
            {
                base.Text = value;
                if (lblTitle != null)
                    lblTitle.Text = value;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            MoveControlsToContentPanel();
        }

        private void MoveControlsToContentPanel()
        {
            Control[] controls = new Control[this.Controls.Count];
            this.Controls.CopyTo(controls, 0);

            foreach (Control control in controls)
            {
                if (control != titleBar && control != ContentPanel)
                {
                    Point originalLocation = control.Location;

                    this.Controls.Remove(control);
                    ContentPanel.Controls.Add(control);

                    control.Location = originalLocation;
                }
            }
        }
    }
}