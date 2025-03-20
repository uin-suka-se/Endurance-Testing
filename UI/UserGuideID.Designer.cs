namespace Endurance_Testing.UI
{
    partial class UserGuideID
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UserGuideID));
            webViewUserGuideID = new Microsoft.Web.WebView2.WinForms.WebView2();
            ((System.ComponentModel.ISupportInitialize)webViewUserGuideID).BeginInit();
            SuspendLayout();
            // 
            // webViewUserGuideID
            // 
            webViewUserGuideID.AllowExternalDrop = true;
            webViewUserGuideID.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            webViewUserGuideID.CreationProperties = null;
            webViewUserGuideID.DefaultBackgroundColor = System.Drawing.Color.White;
            webViewUserGuideID.Location = new System.Drawing.Point(12, 38);
            webViewUserGuideID.Name = "webViewUserGuideID";
            webViewUserGuideID.Size = new System.Drawing.Size(1176, 723);
            webViewUserGuideID.TabIndex = 1;
            webViewUserGuideID.ZoomFactor = 1D;
            // 
            // UserGuideID
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1200, 773);
            Controls.Add(webViewUserGuideID);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MaximumSize = new System.Drawing.Size(1200, 773);
            MinimumSize = new System.Drawing.Size(1200, 773);
            Name = "UserGuideID";
            Text = "Panduan Pengguna";
            Controls.SetChildIndex(webViewUserGuideID, 0);
            ((System.ComponentModel.ISupportInitialize)webViewUserGuideID).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Microsoft.Web.WebView2.WinForms.WebView2 webViewUserGuideID;
    }
}