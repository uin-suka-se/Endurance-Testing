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
            backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            lblApiKey = new System.Windows.Forms.Label();
            textBoxApiKey = new System.Windows.Forms.TextBox();
            comboBoxMode = new System.Windows.Forms.ComboBox();
            lblInputMode = new System.Windows.Forms.Label();
            textBoxInputMaxRequest = new System.Windows.Forms.TextBox();
            textBoxTimeout = new System.Windows.Forms.TextBox();
            lblTimeout = new System.Windows.Forms.Label();
            btnExport = new System.Windows.Forms.Button();
            btnClear = new System.Windows.Forms.Button();
            btnInfo = new System.Windows.Forms.Button();
            btnHelp = new System.Windows.Forms.Button();
            textBoxOutput = new System.Windows.Forms.TextBox();
            lblOutput = new System.Windows.Forms.Label();
            lblTimeLeft = new System.Windows.Forms.Label();
            btnStop = new System.Windows.Forms.Button();
            btnStart = new System.Windows.Forms.Button();
            textBoxTime = new System.Windows.Forms.TextBox();
            lblTime = new System.Windows.Forms.Label();
            textBoxInputRequest = new System.Windows.Forms.TextBox();
            lblInputNumberOfRequest = new System.Windows.Forms.Label();
            panelPeriod = new System.Windows.Forms.Panel();
            radioButtonHour = new System.Windows.Forms.RadioButton();
            radioButtonMinute = new System.Windows.Forms.RadioButton();
            radioButtonSecond = new System.Windows.Forms.RadioButton();
            textBoxInputUrl = new System.Windows.Forms.TextBox();
            lblInputUrl = new System.Windows.Forms.Label();
            lblEnduranceTesting = new System.Windows.Forms.Label();
            panelPeriod.SuspendLayout();
            SuspendLayout();
            // 
            // lblApiKey
            // 
            lblApiKey.AutoSize = true;
            lblApiKey.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            lblApiKey.Location = new System.Drawing.Point(397, 184);
            lblApiKey.Name = "lblApiKey";
            lblApiKey.Size = new System.Drawing.Size(182, 32);
            lblApiKey.TabIndex = 19;
            lblApiKey.Text = "Gemini API Key:";
            // 
            // textBoxApiKey
            // 
            textBoxApiKey.Font = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            textBoxApiKey.Location = new System.Drawing.Point(397, 234);
            textBoxApiKey.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            textBoxApiKey.MaxLength = 999999999;
            textBoxApiKey.Name = "textBoxApiKey";
            textBoxApiKey.Size = new System.Drawing.Size(391, 37);
            textBoxApiKey.TabIndex = 33;
            textBoxApiKey.UseSystemPasswordChar = true;
            textBoxApiKey.TextChanged += textBoxApiKey_TextChanged;
            textBoxApiKey.KeyDown += textBoxApiKey_KeyDown;
            textBoxApiKey.KeyPress += textBoxApiKey_KeyPress;
            textBoxApiKey.MouseDown += textBoxApiKey_MouseDown;
            // 
            // comboBoxMode
            // 
            comboBoxMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comboBoxMode.Font = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            comboBoxMode.FormattingEnabled = true;
            comboBoxMode.Items.AddRange(new object[] { "Stable", "Progressive", "Fluctuative" });
            comboBoxMode.Location = new System.Drawing.Point(1005, 134);
            comboBoxMode.Name = "comboBoxMode";
            comboBoxMode.Size = new System.Drawing.Size(173, 38);
            comboBoxMode.TabIndex = 31;
            comboBoxMode.SelectedIndexChanged += comboBoxMode_SelectedIndexChanged;
            // 
            // lblInputMode
            // 
            lblInputMode.AutoSize = true;
            lblInputMode.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            lblInputMode.Location = new System.Drawing.Point(1005, 88);
            lblInputMode.Name = "lblInputMode";
            lblInputMode.Size = new System.Drawing.Size(82, 32);
            lblInputMode.TabIndex = 26;
            lblInputMode.Text = "Mode:";
            // 
            // textBoxInputMaxRequest
            // 
            textBoxInputMaxRequest.Font = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            textBoxInputMaxRequest.Location = new System.Drawing.Point(794, 135);
            textBoxInputMaxRequest.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            textBoxInputMaxRequest.MaxLength = 999999999;
            textBoxInputMaxRequest.Name = "textBoxInputMaxRequest";
            textBoxInputMaxRequest.Size = new System.Drawing.Size(200, 37);
            textBoxInputMaxRequest.TabIndex = 30;
            textBoxInputMaxRequest.TextChanged += textBoxInputMaxRequest_TextChanged;
            // 
            // textBoxTimeout
            // 
            textBoxTimeout.Font = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            textBoxTimeout.Location = new System.Drawing.Point(27, 234);
            textBoxTimeout.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            textBoxTimeout.MaxLength = 999999999;
            textBoxTimeout.Name = "textBoxTimeout";
            textBoxTimeout.Size = new System.Drawing.Size(364, 37);
            textBoxTimeout.TabIndex = 32;
            textBoxTimeout.TextChanged += textBoxTimeout_TextChanged;
            // 
            // lblTimeout
            // 
            lblTimeout.AutoSize = true;
            lblTimeout.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            lblTimeout.Location = new System.Drawing.Point(27, 184);
            lblTimeout.Name = "lblTimeout";
            lblTimeout.Size = new System.Drawing.Size(364, 32);
            lblTimeout.TabIndex = 27;
            lblTimeout.Text = "Timeout Per-Round (In Seconds):";
            // 
            // btnExport
            // 
            btnExport.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            btnExport.Location = new System.Drawing.Point(1057, 691);
            btnExport.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            btnExport.Name = "btnExport";
            btnExport.Size = new System.Drawing.Size(110, 47);
            btnExport.TabIndex = 42;
            btnExport.Text = "Export";
            btnExport.UseVisualStyleBackColor = true;
            btnExport.Click += btnExport_Click;
            // 
            // btnClear
            // 
            btnClear.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            btnClear.Location = new System.Drawing.Point(1057, 607);
            btnClear.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            btnClear.Name = "btnClear";
            btnClear.Size = new System.Drawing.Size(110, 47);
            btnClear.TabIndex = 41;
            btnClear.Text = "Clear";
            btnClear.UseVisualStyleBackColor = true;
            btnClear.Click += btnClear_Click;
            // 
            // btnInfo
            // 
            btnInfo.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            btnInfo.Location = new System.Drawing.Point(1057, 519);
            btnInfo.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            btnInfo.Name = "btnInfo";
            btnInfo.Size = new System.Drawing.Size(110, 47);
            btnInfo.TabIndex = 40;
            btnInfo.Text = "Info";
            btnInfo.UseVisualStyleBackColor = true;
            btnInfo.Click += btnInfo_Click;
            // 
            // btnHelp
            // 
            btnHelp.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            btnHelp.Location = new System.Drawing.Point(1057, 432);
            btnHelp.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            btnHelp.Name = "btnHelp";
            btnHelp.Size = new System.Drawing.Size(110, 47);
            btnHelp.TabIndex = 39;
            btnHelp.Text = "Help";
            btnHelp.UseVisualStyleBackColor = true;
            btnHelp.Click += btnHelp_Click;
            // 
            // textBoxOutput
            // 
            textBoxOutput.Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            textBoxOutput.Location = new System.Drawing.Point(22, 432);
            textBoxOutput.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            textBoxOutput.MaxLength = 999999999;
            textBoxOutput.Multiline = true;
            textBoxOutput.Name = "textBoxOutput";
            textBoxOutput.ReadOnly = true;
            textBoxOutput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            textBoxOutput.Size = new System.Drawing.Size(1029, 307);
            textBoxOutput.TabIndex = 38;
            // 
            // lblOutput
            // 
            lblOutput.AutoSize = true;
            lblOutput.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            lblOutput.Location = new System.Drawing.Point(22, 384);
            lblOutput.Name = "lblOutput";
            lblOutput.Size = new System.Drawing.Size(95, 32);
            lblOutput.TabIndex = 23;
            lblOutput.Text = "Output:";
            // 
            // lblTimeLeft
            // 
            lblTimeLeft.AutoSize = true;
            lblTimeLeft.Font = new System.Drawing.Font("Segoe UI", 32F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            lblTimeLeft.Location = new System.Drawing.Point(518, 340);
            lblTimeLeft.Name = "lblTimeLeft";
            lblTimeLeft.Size = new System.Drawing.Size(191, 45);
            lblTimeLeft.TabIndex = 22;
            lblTimeLeft.Text = "00:00:00:00";
            // 
            // btnStop
            // 
            btnStop.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            btnStop.Location = new System.Drawing.Point(683, 289);
            btnStop.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            btnStop.Name = "btnStop";
            btnStop.Size = new System.Drawing.Size(110, 47);
            btnStop.TabIndex = 37;
            btnStop.Text = "Stop";
            btnStop.UseVisualStyleBackColor = true;
            btnStop.Click += btnStop_Click;
            // 
            // btnStart
            // 
            btnStart.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            btnStart.Location = new System.Drawing.Point(427, 289);
            btnStart.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            btnStart.Name = "btnStart";
            btnStart.Size = new System.Drawing.Size(110, 47);
            btnStart.TabIndex = 36;
            btnStart.Text = "Start";
            btnStart.UseVisualStyleBackColor = true;
            btnStart.Click += btnStart_Click;
            // 
            // textBoxTime
            // 
            textBoxTime.Font = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            textBoxTime.Location = new System.Drawing.Point(794, 234);
            textBoxTime.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            textBoxTime.MaxLength = 999999999;
            textBoxTime.Name = "textBoxTime";
            textBoxTime.Size = new System.Drawing.Size(200, 37);
            textBoxTime.TabIndex = 34;
            textBoxTime.TextChanged += textBoxTime_TextChanged;
            // 
            // lblTime
            // 
            lblTime.AutoSize = true;
            lblTime.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            lblTime.Location = new System.Drawing.Point(794, 184);
            lblTime.Name = "lblTime";
            lblTime.Size = new System.Drawing.Size(173, 32);
            lblTime.TabIndex = 21;
            lblTime.Text = "Time in Period:";
            // 
            // textBoxInputRequest
            // 
            textBoxInputRequest.Font = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            textBoxInputRequest.Location = new System.Drawing.Point(588, 135);
            textBoxInputRequest.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            textBoxInputRequest.MaxLength = 999999999;
            textBoxInputRequest.Name = "textBoxInputRequest";
            textBoxInputRequest.Size = new System.Drawing.Size(200, 37);
            textBoxInputRequest.TabIndex = 29;
            textBoxInputRequest.TextChanged += textBoxInputRequest_TextChanged;
            // 
            // lblInputNumberOfRequest
            // 
            lblInputNumberOfRequest.AutoSize = true;
            lblInputNumberOfRequest.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            lblInputNumberOfRequest.Location = new System.Drawing.Point(588, 88);
            lblInputNumberOfRequest.Name = "lblInputNumberOfRequest";
            lblInputNumberOfRequest.Size = new System.Drawing.Size(390, 32);
            lblInputNumberOfRequest.TabIndex = 20;
            lblInputNumberOfRequest.Text = "Number of Request (Min and Max):";
            // 
            // panelPeriod
            // 
            panelPeriod.Controls.Add(radioButtonHour);
            panelPeriod.Controls.Add(radioButtonMinute);
            panelPeriod.Controls.Add(radioButtonSecond);
            panelPeriod.Location = new System.Drawing.Point(1005, 188);
            panelPeriod.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            panelPeriod.Name = "panelPeriod";
            panelPeriod.Size = new System.Drawing.Size(174, 83);
            panelPeriod.TabIndex = 35;
            // 
            // radioButtonHour
            // 
            radioButtonHour.AutoSize = true;
            radioButtonHour.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            radioButtonHour.Location = new System.Drawing.Point(3, 60);
            radioButtonHour.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            radioButtonHour.Name = "radioButtonHour";
            radioButtonHour.Size = new System.Drawing.Size(69, 21);
            radioButtonHour.TabIndex = 11;
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
            radioButtonMinute.Size = new System.Drawing.Size(80, 21);
            radioButtonMinute.TabIndex = 10;
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
            radioButtonSecond.Size = new System.Drawing.Size(83, 21);
            radioButtonSecond.TabIndex = 9;
            radioButtonSecond.TabStop = true;
            radioButtonSecond.Text = "Second(s)";
            radioButtonSecond.UseVisualStyleBackColor = true;
            // 
            // textBoxInputUrl
            // 
            textBoxInputUrl.Font = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            textBoxInputUrl.Location = new System.Drawing.Point(27, 134);
            textBoxInputUrl.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            textBoxInputUrl.MaxLength = 999999999;
            textBoxInputUrl.Name = "textBoxInputUrl";
            textBoxInputUrl.Size = new System.Drawing.Size(550, 37);
            textBoxInputUrl.TabIndex = 28;
            textBoxInputUrl.TextChanged += textBoxInputUrl_TextChanged;
            // 
            // lblInputUrl
            // 
            lblInputUrl.AutoSize = true;
            lblInputUrl.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            lblInputUrl.Location = new System.Drawing.Point(27, 88);
            lblInputUrl.Name = "lblInputUrl";
            lblInputUrl.Size = new System.Drawing.Size(60, 32);
            lblInputUrl.TabIndex = 24;
            lblInputUrl.Text = "URL:";
            // 
            // lblEnduranceTesting
            // 
            lblEnduranceTesting.AutoSize = true;
            lblEnduranceTesting.Font = new System.Drawing.Font("Segoe UI", 40F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            lblEnduranceTesting.Location = new System.Drawing.Point(427, 34);
            lblEnduranceTesting.Name = "lblEnduranceTesting";
            lblEnduranceTesting.Size = new System.Drawing.Size(366, 54);
            lblEnduranceTesting.TabIndex = 25;
            lblEnduranceTesting.Text = "Endurance Testing";
            // 
            // EnduranceTesting
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 19F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1200, 773);
            ControlBox = false;
            Controls.Add(lblApiKey);
            Controls.Add(textBoxApiKey);
            Controls.Add(comboBoxMode);
            Controls.Add(lblInputMode);
            Controls.Add(textBoxInputMaxRequest);
            Controls.Add(textBoxTimeout);
            Controls.Add(lblTimeout);
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
            Controls.SetChildIndex(lblEnduranceTesting, 0);
            Controls.SetChildIndex(lblInputUrl, 0);
            Controls.SetChildIndex(textBoxInputUrl, 0);
            Controls.SetChildIndex(panelPeriod, 0);
            Controls.SetChildIndex(lblInputNumberOfRequest, 0);
            Controls.SetChildIndex(textBoxInputRequest, 0);
            Controls.SetChildIndex(lblTime, 0);
            Controls.SetChildIndex(textBoxTime, 0);
            Controls.SetChildIndex(btnStart, 0);
            Controls.SetChildIndex(btnStop, 0);
            Controls.SetChildIndex(lblTimeLeft, 0);
            Controls.SetChildIndex(lblOutput, 0);
            Controls.SetChildIndex(textBoxOutput, 0);
            Controls.SetChildIndex(btnHelp, 0);
            Controls.SetChildIndex(btnInfo, 0);
            Controls.SetChildIndex(btnClear, 0);
            Controls.SetChildIndex(btnExport, 0);
            Controls.SetChildIndex(lblTimeout, 0);
            Controls.SetChildIndex(textBoxTimeout, 0);
            Controls.SetChildIndex(textBoxInputMaxRequest, 0);
            Controls.SetChildIndex(lblInputMode, 0);
            Controls.SetChildIndex(comboBoxMode, 0);
            Controls.SetChildIndex(textBoxApiKey, 0);
            Controls.SetChildIndex(lblApiKey, 0);
            panelPeriod.ResumeLayout(false);
            panelPeriod.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.Label lblApiKey;
        private System.Windows.Forms.TextBox textBoxApiKey;
        private System.Windows.Forms.ComboBox comboBoxMode;
        private System.Windows.Forms.Label lblInputMode;
        private System.Windows.Forms.TextBox textBoxInputMaxRequest;
        private System.Windows.Forms.TextBox textBoxTimeout;
        private System.Windows.Forms.Label lblTimeout;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnInfo;
        private System.Windows.Forms.Button btnHelp;
        private System.Windows.Forms.TextBox textBoxOutput;
        private System.Windows.Forms.Label lblOutput;
        private System.Windows.Forms.Label lblTimeLeft;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.TextBox textBoxTime;
        private System.Windows.Forms.Label lblTime;
        private System.Windows.Forms.TextBox textBoxInputRequest;
        private System.Windows.Forms.Label lblInputNumberOfRequest;
        private System.Windows.Forms.Panel panelPeriod;
        private System.Windows.Forms.RadioButton radioButtonHour;
        private System.Windows.Forms.RadioButton radioButtonMinute;
        private System.Windows.Forms.RadioButton radioButtonSecond;
        private System.Windows.Forms.TextBox textBoxInputUrl;
        private System.Windows.Forms.Label lblInputUrl;
        private System.Windows.Forms.Label lblEnduranceTesting;
    }
}

