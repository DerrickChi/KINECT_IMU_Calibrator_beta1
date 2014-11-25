namespace BluetoothController.UserControls.WinFromUserContorls
{
    partial class CalibratorControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SetupButton = new System.Windows.Forms.Button();
            this.CalibrateButton = new System.Windows.Forms.Button();
            this.OutputTextBox = new System.Windows.Forms.TextBox();
            this.DataProgressBar = new BluetoothController.UserControls.WinFromUserContorls.CalibratorProgressBar();
            this.SuspendLayout();
            // 
            // SetupButton
            // 
            this.SetupButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SetupButton.Location = new System.Drawing.Point(0, 0);
            this.SetupButton.Name = "SetupButton";
            this.SetupButton.Size = new System.Drawing.Size(305, 23);
            this.SetupButton.TabIndex = 0;
            this.SetupButton.Text = "Setup Calibrator";
            this.SetupButton.UseVisualStyleBackColor = true;
            this.SetupButton.Click += new System.EventHandler(this.SetupButton_Click);
            // 
            // CalibrateButton
            // 
            this.CalibrateButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CalibrateButton.Enabled = false;
            this.CalibrateButton.Location = new System.Drawing.Point(0, 58);
            this.CalibrateButton.Name = "CalibrateButton";
            this.CalibrateButton.Size = new System.Drawing.Size(305, 23);
            this.CalibrateButton.TabIndex = 2;
            this.CalibrateButton.Text = "Calibrate";
            this.CalibrateButton.UseVisualStyleBackColor = true;
            this.CalibrateButton.Click += new System.EventHandler(this.CalibrateButton_Click);
            // 
            // OutputTextBox
            // 
            this.OutputTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.OutputTextBox.Enabled = false;
            this.OutputTextBox.Location = new System.Drawing.Point(0, 88);
            this.OutputTextBox.Multiline = true;
            this.OutputTextBox.Name = "OutputTextBox";
            this.OutputTextBox.ReadOnly = true;
            this.OutputTextBox.Size = new System.Drawing.Size(304, 44);
            this.OutputTextBox.TabIndex = 3;
            this.OutputTextBox.Text = "Predicted linear/rotational difference:";
            // 
            // DataProgressBar
            // 
            this.DataProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DataProgressBar.ForeColor = System.Drawing.Color.SpringGreen;
            this.DataProgressBar.Location = new System.Drawing.Point(1, 29);
            this.DataProgressBar.MarqueeAnimationSpeed = 25;
            this.DataProgressBar.Name = "DataProgressBar";
            this.DataProgressBar.Size = new System.Drawing.Size(303, 23);
            this.DataProgressBar.Step = 2;
            this.DataProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.DataProgressBar.TabIndex = 1;
            // 
            // CalibratorControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.OutputTextBox);
            this.Controls.Add(this.CalibrateButton);
            this.Controls.Add(this.DataProgressBar);
            this.Controls.Add(this.SetupButton);
            this.Name = "CalibratorControl";
            this.Size = new System.Drawing.Size(305, 135);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button SetupButton;
        private BluetoothController.UserControls.WinFromUserContorls.CalibratorProgressBar DataProgressBar;
        private System.Windows.Forms.Button CalibrateButton;
        private System.Windows.Forms.TextBox OutputTextBox;
    }
}
