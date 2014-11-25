namespace ControllerTester
{
    partial class Form1
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
            this.BluethoothDevsCheckedListBox = new System.Windows.Forms.CheckedListBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.FileLoggerFolderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            this.LoggingButton = new System.Windows.Forms.Button();
            this.FileLocationTextBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.BrowseButton = new System.Windows.Forms.Button();
            this.elementHost1 = new System.Windows.Forms.Integration.ElementHost();
            this.SensorDataGridView = new System.Windows.Forms.DataGridView();
            this.DeviceName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SensorLocationCol = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.MPS = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DataPreview = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SensorHost = new System.Windows.Forms.Integration.ElementHost();
            this.DefaultOrientButton = new System.Windows.Forms.Button();
            this.QuatArrowHost = new System.Windows.Forms.Integration.ElementHost();
            this.QuatBodyViewerHost = new System.Windows.Forms.Integration.ElementHost();
            this.BindToRightForearmButton = new System.Windows.Forms.Button();
            this.CalibrateLikeFrameButton = new System.Windows.Forms.Button();
            this.Charts = new BluetoothController.SensorGrapherControl();
            this.CalibratorControl = new BluetoothController.UserControls.WinFromUserContorls.CalibratorControl();
            this.LikeFramesTextBox = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.SensorDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // BluethoothDevsCheckedListBox
            // 
            this.BluethoothDevsCheckedListBox.FormattingEnabled = true;
            this.BluethoothDevsCheckedListBox.Location = new System.Drawing.Point(12, 25);
            this.BluethoothDevsCheckedListBox.Name = "BluethoothDevsCheckedListBox";
            this.BluethoothDevsCheckedListBox.Size = new System.Drawing.Size(198, 124);
            this.BluethoothDevsCheckedListBox.TabIndex = 0;
            this.BluethoothDevsCheckedListBox.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.OnItemChecked);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 254);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(198, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "Find Bluetooth Devices";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(384, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Click \"Find Bluetooth Devices\" then check the sensors you would like data from";
            // 
            // FileLoggerFolderBrowser
            // 
            this.FileLoggerFolderBrowser.RootFolder = System.Environment.SpecialFolder.MyDocuments;
            // 
            // LoggingButton
            // 
            this.LoggingButton.Enabled = false;
            this.LoggingButton.Location = new System.Drawing.Point(12, 225);
            this.LoggingButton.Name = "LoggingButton";
            this.LoggingButton.Size = new System.Drawing.Size(198, 23);
            this.LoggingButton.TabIndex = 14;
            this.LoggingButton.Text = "Start Data Logging";
            this.LoggingButton.UseVisualStyleBackColor = true;
            this.LoggingButton.Click += new System.EventHandler(this.LoggingButton_Click);
            // 
            // FileLocationTextBox
            // 
            this.FileLocationTextBox.Location = new System.Drawing.Point(12, 170);
            this.FileLocationTextBox.Name = "FileLocationTextBox";
            this.FileLocationTextBox.ReadOnly = true;
            this.FileLocationTextBox.Size = new System.Drawing.Size(198, 20);
            this.FileLocationTextBox.TabIndex = 15;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(9, 154);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(94, 13);
            this.label5.TabIndex = 16;
            this.label5.Text = "Log File Location :";
            // 
            // BrowseButton
            // 
            this.BrowseButton.Location = new System.Drawing.Point(12, 196);
            this.BrowseButton.Name = "BrowseButton";
            this.BrowseButton.Size = new System.Drawing.Size(198, 23);
            this.BrowseButton.TabIndex = 17;
            this.BrowseButton.Text = "Browse File Location";
            this.BrowseButton.UseVisualStyleBackColor = true;
            this.BrowseButton.Click += new System.EventHandler(this.BrowseButton_Click);
            // 
            // elementHost1
            // 
            this.elementHost1.Enabled = false;
            this.elementHost1.Location = new System.Drawing.Point(12, 283);
            this.elementHost1.MaximumSize = new System.Drawing.Size(300, 300);
            this.elementHost1.Name = "elementHost1";
            this.elementHost1.Size = new System.Drawing.Size(198, 166);
            this.elementHost1.TabIndex = 0;
            this.elementHost1.Text = "elementHost1";
            this.elementHost1.Child = null;
            // 
            // SensorDataGridView
            // 
            this.SensorDataGridView.AllowUserToAddRows = false;
            this.SensorDataGridView.AllowUserToDeleteRows = false;
            this.SensorDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.SensorDataGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders;
            this.SensorDataGridView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.SensorDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.SensorDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.DeviceName,
            this.SensorLocationCol,
            this.MPS,
            this.DataPreview});
            this.SensorDataGridView.ImeMode = System.Windows.Forms.ImeMode.On;
            this.SensorDataGridView.Location = new System.Drawing.Point(12, 511);
            this.SensorDataGridView.MultiSelect = false;
            this.SensorDataGridView.Name = "SensorDataGridView";
            this.SensorDataGridView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.SensorDataGridView.RowHeadersVisible = false;
            this.SensorDataGridView.ShowCellErrors = false;
            this.SensorDataGridView.ShowCellToolTips = false;
            this.SensorDataGridView.Size = new System.Drawing.Size(764, 162);
            this.SensorDataGridView.TabIndex = 22;
            this.SensorDataGridView.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.SensorDataGridView_CellBeginEdit);
            this.SensorDataGridView.CurrentCellDirtyStateChanged += new System.EventHandler(this.SensorDataGridView_CurrentCellDirtyStateChanged);
            this.SensorDataGridView.Click += new System.EventHandler(this.SensorDataGridView_Click);
            this.SensorDataGridView.MouseEnter += new System.EventHandler(this.SensorDataGridView_Click);
            this.SensorDataGridView.MouseLeave += new System.EventHandler(this.SensorDataGridView_CurrentCellDirtyStateChanged);
            // 
            // DeviceName
            // 
            this.DeviceName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.DeviceName.HeaderText = "Device Name";
            this.DeviceName.Name = "DeviceName";
            this.DeviceName.Width = 89;
            // 
            // SensorLocationCol
            // 
            this.SensorLocationCol.HeaderText = "Sensor Location";
            this.SensorLocationCol.Name = "SensorLocationCol";
            this.SensorLocationCol.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.SensorLocationCol.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // MPS
            // 
            this.MPS.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.MPS.HeaderText = "MPS";
            this.MPS.Name = "MPS";
            this.MPS.Width = 55;
            // 
            // DataPreview
            // 
            this.DataPreview.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.DataPreview.HeaderText = "Data Preview";
            this.DataPreview.Name = "DataPreview";
            // 
            // SensorHost
            // 
            this.SensorHost.Location = new System.Drawing.Point(221, 215);
            this.SensorHost.Name = "SensorHost";
            this.SensorHost.Size = new System.Drawing.Size(172, 142);
            this.SensorHost.TabIndex = 23;
            this.SensorHost.Text = "elementHost2";
            this.SensorHost.Child = null;
            // 
            // DefaultOrientButton
            // 
            this.DefaultOrientButton.Location = new System.Drawing.Point(12, 453);
            this.DefaultOrientButton.Name = "DefaultOrientButton";
            this.DefaultOrientButton.Size = new System.Drawing.Size(198, 23);
            this.DefaultOrientButton.TabIndex = 24;
            this.DefaultOrientButton.Text = "Bind To Kinect Default Cord";
            this.DefaultOrientButton.UseVisualStyleBackColor = true;
            this.DefaultOrientButton.Click += new System.EventHandler(this.DefaultOrientButton_Click);
            // 
            // QuatArrowHost
            // 
            this.QuatArrowHost.Location = new System.Drawing.Point(224, 363);
            this.QuatArrowHost.Name = "QuatArrowHost";
            this.QuatArrowHost.Size = new System.Drawing.Size(169, 142);
            this.QuatArrowHost.TabIndex = 25;
            this.QuatArrowHost.Text = "elementHost2";
            this.QuatArrowHost.Child = null;
            // 
            // QuatBodyViewerHost
            // 
            this.QuatBodyViewerHost.Location = new System.Drawing.Point(412, 25);
            this.QuatBodyViewerHost.Name = "QuatBodyViewerHost";
            this.QuatBodyViewerHost.Size = new System.Drawing.Size(364, 290);
            this.QuatBodyViewerHost.TabIndex = 26;
            this.QuatBodyViewerHost.Text = "QuatBodyHost";
            this.QuatBodyViewerHost.Child = null;
            // 
            // BindToRightForearmButton
            // 
            this.BindToRightForearmButton.Location = new System.Drawing.Point(12, 482);
            this.BindToRightForearmButton.Name = "BindToRightForearmButton";
            this.BindToRightForearmButton.Size = new System.Drawing.Size(198, 23);
            this.BindToRightForearmButton.TabIndex = 28;
            this.BindToRightForearmButton.Text = "Bind to Right Forearm";
            this.BindToRightForearmButton.UseVisualStyleBackColor = true;
            this.BindToRightForearmButton.Click += new System.EventHandler(this.BindToRightForearmButton_Click);
            // 
            // CalibrateLikeFrameButton
            // 
            this.CalibrateLikeFrameButton.Location = new System.Drawing.Point(412, 443);
            this.CalibrateLikeFrameButton.Name = "CalibrateLikeFrameButton";
            this.CalibrateLikeFrameButton.Size = new System.Drawing.Size(172, 23);
            this.CalibrateLikeFrameButton.TabIndex = 30;
            this.CalibrateLikeFrameButton.Text = "Calibrate LIke Frames";
            this.CalibrateLikeFrameButton.UseVisualStyleBackColor = true;
            this.CalibrateLikeFrameButton.Click += new System.EventHandler(this.CalibrateLikeFrameButton_Click);
            // 
            // Charts
            // 
            this.Charts.Location = new System.Drawing.Point(782, 25);
            this.Charts.Name = "Charts";
            this.Charts.Size = new System.Drawing.Size(288, 648);
            this.Charts.TabIndex = 27;
            // 
            // CalibratorControl
            // 
            this.CalibratorControl.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.CalibratorControl.InertialSesnor = null;
            this.CalibratorControl.Location = new System.Drawing.Point(216, 25);
            this.CalibratorControl.Name = "CalibratorControl";
            this.CalibratorControl.Size = new System.Drawing.Size(190, 184);
            this.CalibratorControl.TabIndex = 29;
            this.CalibratorControl.VirtualSesnor = null;
            // 
            // LikeFramesTextBox
            // 
            this.LikeFramesTextBox.Location = new System.Drawing.Point(412, 321);
            this.LikeFramesTextBox.Multiline = true;
            this.LikeFramesTextBox.Name = "LikeFramesTextBox";
            this.LikeFramesTextBox.Size = new System.Drawing.Size(172, 116);
            this.LikeFramesTextBox.TabIndex = 31;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1078, 680);
            this.Controls.Add(this.LikeFramesTextBox);
            this.Controls.Add(this.CalibrateLikeFrameButton);
            this.Controls.Add(this.BindToRightForearmButton);
            this.Controls.Add(this.Charts);
            this.Controls.Add(this.QuatBodyViewerHost);
            this.Controls.Add(this.QuatArrowHost);
            this.Controls.Add(this.DefaultOrientButton);
            this.Controls.Add(this.SensorHost);
            this.Controls.Add(this.SensorDataGridView);
            this.Controls.Add(this.elementHost1);
            this.Controls.Add(this.BrowseButton);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.FileLocationTextBox);
            this.Controls.Add(this.LoggingButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.BluethoothDevsCheckedListBox);
            this.Controls.Add(this.CalibratorControl);
            this.Name = "Form1";
            this.Text = "Bluetooth Sensor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load_1);
            this.Shown += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.SensorDataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckedListBox BluethoothDevsCheckedListBox;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.FolderBrowserDialog FileLoggerFolderBrowser;
        private System.Windows.Forms.Button LoggingButton;
        private System.Windows.Forms.TextBox FileLocationTextBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button BrowseButton;
        private System.Windows.Forms.Integration.ElementHost elementHost1;
        private System.Windows.Forms.DataGridView SensorDataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn DeviceName;
        private System.Windows.Forms.DataGridViewComboBoxColumn SensorLocationCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn MPS;
        private System.Windows.Forms.DataGridViewTextBoxColumn DataPreview;
        private System.Windows.Forms.Integration.ElementHost SensorHost;
        private System.Windows.Forms.Button DefaultOrientButton;
        private System.Windows.Forms.Integration.ElementHost QuatArrowHost;
        private System.Windows.Forms.Integration.ElementHost QuatBodyViewerHost;
        private BluetoothController.SensorGrapherControl Charts;
        private System.Windows.Forms.Button BindToRightForearmButton;
        private BluetoothController.UserControls.WinFromUserContorls.CalibratorControl CalibratorControl;
        private System.Windows.Forms.Button CalibrateLikeFrameButton;
        private System.Windows.Forms.TextBox LikeFramesTextBox;
    }
}

