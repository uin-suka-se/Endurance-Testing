namespace Endurance_Testing.UI
{
    partial class UserGuideEN
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UserGuideEN));
            webViewUserGuideEN = new Microsoft.Web.WebView2.WinForms.WebView2();
            ((System.ComponentModel.ISupportInitialize)webViewUserGuideEN).BeginInit();
            SuspendLayout();
            // 
            // webViewUserGuideEN
            // 
            webViewUserGuideEN.AllowExternalDrop = true;
            webViewUserGuideEN.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            webViewUserGuideEN.CreationProperties = null;
            webViewUserGuideEN.DefaultBackgroundColor = System.Drawing.Color.White;
            webViewUserGuideEN.Location = new System.Drawing.Point(12, 38);
            webViewUserGuideEN.Name = "webViewUserGuideEN";
            webViewUserGuideEN.Size = new System.Drawing.Size(1176, 723);
            webViewUserGuideEN.TabIndex = 1;
            webViewUserGuideEN.ZoomFactor = 1D;
            // 
            // UserGuideEN
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1200, 773);
            Controls.Add(webViewUserGuideEN);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MaximumSize = new System.Drawing.Size(1200, 773);
            MinimumSize = new System.Drawing.Size(1200, 773);
            Name = "UserGuideEN";
            Text = "User Guide";
            Controls.SetChildIndex(webViewUserGuideEN, 0);
            ((System.ComponentModel.ISupportInitialize)webViewUserGuideEN).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Microsoft.Web.WebView2.WinForms.WebView2 webViewUserGuideEN;
    }
}