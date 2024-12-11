namespace Endurance_Testing
{
    partial class EnduranceTesting
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EnduranceTesting));
            lblEnduranceTesting = new System.Windows.Forms.Label();
            lblInputUrl = new System.Windows.Forms.Label();
            textBoxInputUrl = new System.Windows.Forms.TextBox();
            panelPeriod = new System.Windows.Forms.Panel();
            radioButtonHour = new System.Windows.Forms.RadioButton();
            radioButtonMinute = new System.Windows.Forms.RadioButton();
            radioButtonSecond = new System.Windows.Forms.RadioButton();
            textBoxInputRequest = new System.Windows.Forms.TextBox();
            lblInputNumberOfRequest = new System.Windows.Forms.Label();
            textBoxTime = new System.Windows.Forms.TextBox();
            lblTime = new System.Windows.Forms.Label();
            btnStart = new System.Windows.Forms.Button();
            btnStop = new System.Windows.Forms.Button();
            lblTimeLeft = new System.Windows.Forms.Label();
            lblOutput = new System.Windows.Forms.Label();
            textBoxOutput = new System.Windows.Forms.TextBox();
            btnHelp = new System.Windows.Forms.Button();
            btnInfo = new System.Windows.Forms.Button();
            btnClear = new System.Windows.Forms.Button();
            btnExport = new System.Windows.Forms.Button();
            backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            panelPeriod.SuspendLayout();
            SuspendLayout();
            // 
            // lblEnduranceTesting
            // 
            lblEnduranceTesting.AutoSize = true;
            lblEnduranceTesting.Font = new System.Drawing.Font("Segoe UI", 40F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            lblEnduranceTesting.Location = new System.Drawing.Point(418, 24);
            lblEnduranceTesting.Name = "lblEnduranceTesting";
            lblEnduranceTesting.Size = new System.Drawing.Size(366, 54);
            lblEnduranceTesting.TabIndex = 0;
            lblEnduranceTesting.Text = "Endurance Testing";
            // 
            // lblInputUrl
            // 
            lblInputUrl.AutoSize = true;
            lblInputUrl.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            lblInputUrl.Location = new System.Drawing.Point(13, 105);
            lblInputUrl.Name = "lblInputUrl";
            lblInputUrl.Size = new System.Drawing.Size(129, 35);
            lblInputUrl.TabIndex = 0;
            lblInputUrl.Text = "Input URL:";
            // 
            // textBoxInputUrl
            // 
            textBoxInputUrl.Font = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            textBoxInputUrl.Location = new System.Drawing.Point(13, 151);
            textBoxInputUrl.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            textBoxInputUrl.MaxLength = 999999999;
            textBoxInputUrl.Name = "textBoxInputUrl";
            textBoxInputUrl.Size = new System.Drawing.Size(348, 37);
            textBoxInputUrl.TabIndex = 1;
            textBoxInputUrl.TextChanged += textBoxInputUrl_TextChanged;
            // 
            // panelPeriod
            // 
            panelPeriod.Controls.Add(radioButtonHour);
            panelPeriod.Controls.Add(radioButtonMinute);
            panelPeriod.Controls.Add(radioButtonSecond);
            panelPeriod.Location = new System.Drawing.Point(1080, 105);
            panelPeriod.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            panelPeriod.Name = "panelPeriod";
            panelPeriod.Size = new System.Drawing.Size(90, 83);
            panelPeriod.TabIndex = 4;
            // 
            // radioButtonHour
            // 
            radioButtonHour.AutoSize = true;
            radioButtonHour.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            radioButtonHour.Location = new System.Drawing.Point(3, 60);
            radioButtonHour.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            radioButtonHour.Name = "radioButtonHour";
            radioButtonHour.Size = new System.Drawing.Size(72, 21);
            radioButtonHour.TabIndex = 6;
            radioButtonHour.TabStop = true;
            radioButtonHour.Text = "Hour(s)";
            radioButtonHour.UseVisualStyleBackColor = true;
            // 
            // radioButtonMinute
            // 
            radioButtonMinute.AutoSize = true;
            radioButtonMinute.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            radioButtonMinute.Location = new System.Drawing.Point(3, 32);
            radioButtonMinute.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            radioButtonMinute.Name = "radioButtonMinute";
            radioButtonMinute.Size = new System.Drawing.Size(83, 21);
            radioButtonMinute.TabIndex = 5;
            radioButtonMinute.TabStop = true;
            radioButtonMinute.Text = "Minute(s)";
            radioButtonMinute.UseVisualStyleBackColor = true;
            // 
            // radioButtonSecond
            // 
            radioButtonSecond.AutoSize = true;
            radioButtonSecond.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            radioButtonSecond.Location = new System.Drawing.Point(3, 4);
            radioButtonSecond.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            radioButtonSecond.Name = "radioButtonSecond";
            radioButtonSecond.Size = new System.Drawing.Size(86, 21);
            radioButtonSecond.TabIndex = 4;
            radioButtonSecond.TabStop = true;
            radioButtonSecond.Text = "Second(s)";
            radioButtonSecond.UseVisualStyleBackColor = true;
            // 
            // textBoxInputRequest
            // 
            textBoxInputRequest.Font = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            textBoxInputRequest.Location = new System.Drawing.Point(369, 151);
            textBoxInputRequest.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            textBoxInputRequest.MaxLength = 999999999;
            textBoxInputRequest.Name = "textBoxInputRequest";
            textBoxInputRequest.Size = new System.Drawing.Size(348, 37);
            textBoxInputRequest.TabIndex = 2;
            textBoxInputRequest.TextChanged += textBoxInputRequest_TextChanged;
            // 
            // lblInputNumberOfRequest
            // 
            lblInputNumberOfRequest.AutoSize = true;
            lblInputNumberOfRequest.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            lblInputNumberOfRequest.Location = new System.Drawing.Point(369, 105);
            lblInputNumberOfRequest.Name = "lblInputNumberOfRequest";
            lblInputNumberOfRequest.Size = new System.Drawing.Size(302, 35);
            lblInputNumberOfRequest.TabIndex = 0;
            lblInputNumberOfRequest.Text = "Input Number of Request:";
            // 
            // textBoxTime
            // 
            textBoxTime.Font = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            textBoxTime.Location = new System.Drawing.Point(726, 151);
            textBoxTime.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            textBoxTime.MaxLength = 999999999;
            textBoxTime.Name = "textBoxTime";
            textBoxTime.Size = new System.Drawing.Size(348, 37);
            textBoxTime.TabIndex = 3;
            textBoxTime.TextChanged += textBoxTime_TextChanged;
            // 
            // lblTime
            // 
            lblTime.AutoSize = true;
            lblTime.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            lblTime.Location = new System.Drawing.Point(726, 105);
            lblTime.Name = "lblTime";
            lblTime.Size = new System.Drawing.Size(244, 35);
            lblTime.TabIndex = 0;
            lblTime.Text = "Input Time in Period:";
            // 
            // btnStart
            // 
            btnStart.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            btnStart.Location = new System.Drawing.Point(418, 217);
            btnStart.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            btnStart.Name = "btnStart";
            btnStart.Size = new System.Drawing.Size(110, 47);
            btnStart.TabIndex = 7;
            btnStart.Text = "Start";
            btnStart.UseVisualStyleBackColor = true;
            btnStart.Click += btnStart_Click;
            // 
            // btnStop
            // 
            btnStop.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            btnStop.Location = new System.Drawing.Point(674, 217);
            btnStop.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            btnStop.Name = "btnStop";
            btnStop.Size = new System.Drawing.Size(110, 47);
            btnStop.TabIndex = 8;
            btnStop.Text = "Stop";
            btnStop.UseVisualStyleBackColor = true;
            btnStop.Click += btnStop_Click;
            // 
            // lblTimeLeft
            // 
            lblTimeLeft.AutoSize = true;
            lblTimeLeft.Font = new System.Drawing.Font("Segoe UI", 32F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            lblTimeLeft.Location = new System.Drawing.Point(511, 295);
            lblTimeLeft.Name = "lblTimeLeft";
            lblTimeLeft.Size = new System.Drawing.Size(177, 45);
            lblTimeLeft.TabIndex = 0;
            lblTimeLeft.Text = "00:00:00:00";
            // 
            // lblOutput
            // 
            lblOutput.AutoSize = true;
            lblOutput.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            lblOutput.Location = new System.Drawing.Point(13, 362);
            lblOutput.Name = "lblOutput";
            lblOutput.Size = new System.Drawing.Size(98, 35);
            lblOutput.TabIndex = 0;
            lblOutput.Text = "Output:";
            // 
            // textBoxOutput
            // 
            textBoxOutput.Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            textBoxOutput.Location = new System.Drawing.Point(13, 407);
            textBoxOutput.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            textBoxOutput.MaxLength = 999999999;
            textBoxOutput.Multiline = true;
            textBoxOutput.Name = "textBoxOutput";
            textBoxOutput.ReadOnly = true;
            textBoxOutput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            textBoxOutput.Size = new System.Drawing.Size(1029, 307);
            textBoxOutput.TabIndex = 9;
            // 
            // btnHelp
            // 
            btnHelp.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            btnHelp.Location = new System.Drawing.Point(1048, 407);
            btnHelp.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            btnHelp.Name = "btnHelp";
            btnHelp.Size = new System.Drawing.Size(110, 47);
            btnHelp.TabIndex = 10;
            btnHelp.Text = "Help";
            btnHelp.UseVisualStyleBackColor = true;
            btnHelp.Click += btnHelp_Click;
            // 
            // btnInfo
            // 
            btnInfo.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            btnInfo.Location = new System.Drawing.Point(1048, 494);
            btnInfo.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            btnInfo.Name = "btnInfo";
            btnInfo.Size = new System.Drawing.Size(110, 47);
            btnInfo.TabIndex = 11;
            btnInfo.Text = "Info";
            btnInfo.UseVisualStyleBackColor = true;
            btnInfo.Click += btnInfo_Click;
            // 
            // btnClear
            // 
            btnClear.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            btnClear.Location = new System.Drawing.Point(1048, 582);
            btnClear.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            btnClear.Name = "btnClear";
            btnClear.Size = new System.Drawing.Size(110, 47);
            btnClear.TabIndex = 12;
            btnClear.Text = "Clear";
            btnClear.UseVisualStyleBackColor = true;
            btnClear.Click += btnClear_Click;
            // 
            // btnExport
            // 
            btnExport.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            btnExport.Location = new System.Drawing.Point(1048, 666);
            btnExport.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            btnExport.Name = "btnExport";
            btnExport.Size = new System.Drawing.Size(110, 47);
            btnExport.TabIndex = 13;
            btnExport.Text = "Export";
            btnExport.UseVisualStyleBackColor = true;
            btnExport.Click += btnExport_Click;
            // 
            // EnduranceTesting
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 19F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1182, 726);
            Controls.Add(btnExport);
            Controls.Add(btnClear);
            Controls.Add(btnInfo);
            Controls.Add(btnHelp);
            Controls.Add(textBoxOutput);
            Controls.Add(lblOutput);
            Controls.Add(lblTimeLeft);
            Controls.Add(btnStop);
            Controls.Add(btnStart);
            Controls.Add(textBoxTime);
            Controls.Add(lblTime);
            Controls.Add(textBoxInputRequest);
            Controls.Add(lblInputNumberOfRequest);
            Controls.Add(panelPeriod);
            Controls.Add(textBoxInputUrl);
            Controls.Add(lblInputUrl);
            Controls.Add(lblEnduranceTesting);
            Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            MaximizeBox = false;
            MaximumSize = new System.Drawing.Size(1200, 773);
            MinimumSize = new System.Drawing.Size(1200, 773);
            Name = "EnduranceTesting";
            Text = "Endurance Testing";
            panelPeriod.ResumeLayout(false);
            panelPeriod.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblEnduranceTesting;
        private System.Windows.Forms.Label lblInputUrl;
        private System.Windows.Forms.TextBox textBoxInputUrl;
        private System.Windows.Forms.Panel panelPeriod;
        private System.Windows.Forms.RadioButton radioButtonHour;
        private System.Windows.Forms.RadioButton radioButtonMinute;
        private System.Windows.Forms.RadioButton radioButtonSecond;
        private System.Windows.Forms.TextBox textBoxInputRequest;
        private System.Windows.Forms.Label lblInputNumberOfRequest;
        private System.Windows.Forms.TextBox textBoxTime;
        private System.Windows.Forms.Label lblTime;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Label lblTimeLeft;
        private System.Windows.Forms.Label lblOutput;
        private System.Windows.Forms.TextBox textBoxOutput;
        private System.Windows.Forms.Button btnHelp;
        private System.Windows.Forms.Button btnInfo;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnExport;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
    }
}

