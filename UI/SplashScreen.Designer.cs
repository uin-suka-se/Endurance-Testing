namespace Endurance_Testing.UI
{
    partial class SplashScreen
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SplashScreen));
            circularProgressBar = new CircularProgressBar_NET5.CircularProgressBar();
            lblAppName = new System.Windows.Forms.Label();
            lblLoading = new System.Windows.Forms.Label();
            lblMade = new System.Windows.Forms.Label();
            timerSplashScreen = new System.Windows.Forms.Timer(components);
            SuspendLayout();
            // 
            // circularProgressBar
            // 
            circularProgressBar.AnimationFunction = WinFormAnimation_NET5.KnownAnimationFunctions.Linear;
            circularProgressBar.AnimationSpeed = 0;
            circularProgressBar.BackColor = System.Drawing.Color.FromArgb(42, 40, 60);
            circularProgressBar.Font = new System.Drawing.Font("Segoe UI", 26.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            circularProgressBar.ForeColor = System.Drawing.Color.FromArgb(85, 213, 219);
            circularProgressBar.InnerColor = System.Drawing.Color.FromArgb(42, 40, 60);
            circularProgressBar.InnerMargin = 2;
            circularProgressBar.InnerWidth = -1;
            circularProgressBar.Location = new System.Drawing.Point(56, 112);
            circularProgressBar.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            circularProgressBar.MarqueeAnimationSpeed = 2000;
            circularProgressBar.Name = "circularProgressBar";
            circularProgressBar.OuterColor = System.Drawing.Color.FromArgb(28, 26, 43);
            circularProgressBar.OuterMargin = -25;
            circularProgressBar.OuterWidth = 26;
            circularProgressBar.ProgressColor = System.Drawing.Color.FromArgb(85, 213, 219);
            circularProgressBar.ProgressWidth = 6;
            circularProgressBar.SecondaryFont = new System.Drawing.Font("Segoe UI", 36F);
            circularProgressBar.Size = new System.Drawing.Size(150, 150);
            circularProgressBar.StartAngle = 270;
            circularProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            circularProgressBar.SubscriptColor = System.Drawing.Color.FromArgb(166, 166, 166);
            circularProgressBar.SubscriptMargin = new System.Windows.Forms.Padding(10, -35, 0, 0);
            circularProgressBar.SubscriptText = "";
            circularProgressBar.SuperscriptColor = System.Drawing.Color.FromArgb(166, 166, 166);
            circularProgressBar.SuperscriptMargin = new System.Windows.Forms.Padding(10, 35, 0, 0);
            circularProgressBar.SuperscriptText = "";
            circularProgressBar.TabIndex = 0;
            circularProgressBar.TextMargin = new System.Windows.Forms.Padding(8, 8, 0, 0);
            circularProgressBar.Value = 68;
            // 
            // lblAppName
            // 
            lblAppName.AutoSize = true;
            lblAppName.Font = new System.Drawing.Font("Simplified Arabic", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            lblAppName.ForeColor = System.Drawing.Color.FromArgb(85, 213, 219);
            lblAppName.Location = new System.Drawing.Point(13, 49);
            lblAppName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblAppName.Name = "lblAppName";
            lblAppName.Size = new System.Drawing.Size(244, 45);
            lblAppName.TabIndex = 1;
            lblAppName.Text = "Endurance Testing";
            // 
            // lblLoading
            // 
            lblLoading.AutoSize = true;
            lblLoading.Font = new System.Drawing.Font("Simplified Arabic", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            lblLoading.ForeColor = System.Drawing.Color.FromArgb(85, 213, 219);
            lblLoading.Location = new System.Drawing.Point(84, 286);
            lblLoading.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblLoading.Name = "lblLoading";
            lblLoading.Size = new System.Drawing.Size(97, 32);
            lblLoading.TabIndex = 2;
            lblLoading.Text = "Loading...";
            // 
            // lblMade
            // 
            lblMade.AutoSize = true;
            lblMade.Font = new System.Drawing.Font("Simplified Arabic", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            lblMade.ForeColor = System.Drawing.Color.FromArgb(85, 213, 219);
            lblMade.Location = new System.Drawing.Point(37, 342);
            lblMade.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblMade.Name = "lblMade";
            lblMade.Size = new System.Drawing.Size(201, 23);
            lblMade.TabIndex = 3;
            lblMade.Text = "Made by Rahma Bintang Pratama";
            // 
            // timerSplashScreen
            // 
            timerSplashScreen.Enabled = true;
            timerSplashScreen.Interval = 25;
            timerSplashScreen.Tick += timerSplashScreen_Tick;
            // 
            // SplashScreen
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.FromArgb(42, 40, 60);
            ClientSize = new System.Drawing.Size(270, 390);
            Controls.Add(lblMade);
            Controls.Add(lblLoading);
            Controls.Add(lblAppName);
            Controls.Add(circularProgressBar);
            Cursor = System.Windows.Forms.Cursors.WaitCursor;
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MaximumSize = new System.Drawing.Size(270, 390);
            MinimizeBox = false;
            MinimumSize = new System.Drawing.Size(270, 390);
            Name = "SplashScreen";
            Opacity = 0.88D;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "SplashScreen";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private CircularProgressBar_NET5.CircularProgressBar circularProgressBar;
        private System.Windows.Forms.Label lblAppName;
        private System.Windows.Forms.Label lblLoading;
        private System.Windows.Forms.Label lblMade;
        private System.Windows.Forms.Timer timerSplashScreen;
    }
}