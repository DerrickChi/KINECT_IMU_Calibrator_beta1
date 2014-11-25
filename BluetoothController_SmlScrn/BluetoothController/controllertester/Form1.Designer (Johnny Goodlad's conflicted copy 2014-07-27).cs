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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series4 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title1 = new System.Windows.Forms.DataVisualization.Charting.Title();
            this.checkedListBox1 = new System.Windows.Forms.CheckedListBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.AccelChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
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
            this.Charts = new BluetoothController.SensorGrapher();
            ((System.ComponentModel.ISupportInitialize)(this.AccelChart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SensorDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // checkedListBox1
            // 
            this.checkedListBox1.FormattingEnabled = true;
            this.checkedListBox1.Location = new System.Drawing.Point(12, 25);
            this.checkedListBox1.Name = "checkedListBox1";
            this.checkedListBox1.Size = new System.Drawing.Size(231, 154);
            this.checkedListBox1.TabIndex = 0;
            this.checkedListBox1.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.OnItemChecked);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 297);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(231, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "Find Bluetooth Devices";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(384, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Click \"Find Bluetooth Devices\" then check the sensors you would like data from";
            // 
            // AccelChart
            // 
            this.AccelChart.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            chartArea1.Name = "ChartArea1";
            chartArea2.Name = "ChartArea2";
            this.AccelChart.ChartAreas.Add(chartArea1);
            this.AccelChart.ChartAreas.Add(chartArea2);
            this.AccelChart.Enabled = false;
            legend1.Alignment = System.Drawing.StringAlignment.Far;
            legend1.BackColor = System.Drawing.SystemColors.InactiveCaption;
            legend1.DockedToChartArea = "ChartArea1";
            legend1.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Left;
            legend1.LegendStyle = System.Windows.Forms.DataVisualization.Charting.LegendStyle.Column;
            legend1.Name = "Legend1";
            legend1.TableStyle = System.Windows.Forms.DataVisualization.Charting.LegendTableStyle.Wide;
            legend1.TitleBackColor = System.Drawing.Color.White;
            legend2.DockedToChartArea = "ChartArea2";
            legend2.Name = "Legend2";
            this.AccelChart.Legends.Add(legend1);
            this.AccelChart.Legends.Add(legend2);
            this.AccelChart.Location = new System.Drawing.Point(1037, 12);
            this.AccelChart.Name = "AccelChart";
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series1.Legend = "Legend1";
            series1.Name = "Accel X";
            series2.ChartArea = "ChartArea1";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series2.Legend = "Legend1";
            series2.Name = "Accel Y";
            series3.ChartArea = "ChartArea1";
            series3.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series3.Legend = "Legend1";
            series3.Name = "Accel Z";
            series4.ChartArea = "ChartArea2";
            series4.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series4.Legend = "Legend2";
            series4.Name = "accel2y";
            this.AccelChart.Series.Add(series1);
            this.AccelChart.Series.Add(series2);
            this.AccelChart.Series.Add(series3);
            this.AccelChart.Series.Add(series4);
            this.AccelChart.Size = new System.Drawing.Size(302, 769);
            this.AccelChart.TabIndex = 10;
            this.AccelChart.Text = "chart1";
            title1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            title1.DockedToChartArea = "ChartArea1";
            title1.Name = "Title1";
            title1.Text = "My Title";
            this.AccelChart.Titles.Add(title1);
            this.AccelChart.Visible = false;
            // 
            // FileLoggerFolderBrowser
            // 
            this.FileLoggerFolderBrowser.RootFolder = System.Environment.SpecialFolder.MyDocuments;
            // 
            // LoggingButton
            // 
            this.LoggingButton.Enabled = false;
            this.LoggingButton.Location = new System.Drawing.Point(12, 268);
            this.LoggingButton.Name = "LoggingButton";
            this.LoggingButton.Size = new System.Drawing.Size(231, 23);
            this.LoggingButton.TabIndex = 14;
            this.LoggingButton.Text = "Start Data Logging";
            this.LoggingButton.UseVisualStyleBackColor = true;
            this.LoggingButton.Click += new System.EventHandler(this.LoggingButton_Click);
            // 
            // FileLocationTextBox
            // 
            this.FileLocationTextBox.Location = new System.Drawing.Point(12, 213);
            this.FileLocationTextBox.Name = "FileLocationTextBox";
            this.FileLocationTextBox.ReadOnly = true;
            this.FileLocationTextBox.Size = new System.Drawing.Size(231, 20);
            this.FileLocationTextBox.TabIndex = 15;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(9, 197);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(94, 13);
            this.label5.TabIndex = 16;
            this.label5.Text = "Log File Location :";
            // 
            // BrowseButton
            // 
            this.BrowseButton.Location = new System.Drawing.Point(12, 239);
            this.BrowseButton.Name = "BrowseButton";
            this.BrowseButton.Size = new System.Drawing.Size(231, 23);
            this.BrowseButton.TabIndex = 17;
            this.BrowseButton.Text = "Browse File Location";
            this.BrowseButton.UseVisualStyleBackColor = true;
            this.BrowseButton.Click += new System.EventHandler(this.BrowseButton_Click);
            // 
            // elementHost1
            // 
            this.elementHost1.Enabled = false;
            this.elementHost1.Location = new System.Drawing.Point(12, 326);
            this.elementHost1.MaximumSize = new System.Drawing.Size(300, 300);
            this.elementHost1.Name = "elementHost1";
            this.elementHost1.Size = new System.Drawing.Size(231, 189);
            this.elementHost1.TabIndex = 0;
            this.elementHost1.Text = "elementHost1";
            this.elementHost1.Child = null;
            // 
            // SensorDataGridView
            // 
            this.SensorDataGridView.AllowUserToAddRows = false;
            this.SensorDataGridView.AllowUserToDeleteRows = false;
            this.SensorDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
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
            this.SensorDataGridView.Location = new System.Drawing.Point(4, 593);
            this.SensorDataGridView.MultiSelect = false;
            this.SensorDataGridView.Name = "SensorDataGridView";
            this.SensorDataGridView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.SensorDataGridView.RowHeadersVisible = false;
            this.SensorDataGridView.ShowCellErrors = false;
            this.SensorDataGridView.ShowCellToolTips = false;
            this.SensorDataGridView.Size = new System.Drawing.Size(1027, 188);
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
            this.SensorHost.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SensorHost.Location = new System.Drawing.Point(761, 9);
            this.SensorHost.Name = "SensorHost";
            this.SensorHost.Size = new System.Drawing.Size(270, 194);
            this.SensorHost.TabIndex = 23;
            this.SensorHost.Text = "elementHost2";
            this.SensorHost.Child = null;
            // 
            // DefaultOrientButton
            // 
            this.DefaultOrientButton.Location = new System.Drawing.Point(12, 521);
            this.DefaultOrientButton.Name = "DefaultOrientButton";
            this.DefaultOrientButton.Size = new System.Drawing.Size(231, 23);
            this.DefaultOrientButton.TabIndex = 24;
            this.DefaultOrientButton.Text = "Set Default Orientation to Kinect Cord";
            this.DefaultOrientButton.UseVisualStyleBackColor = true;
            this.DefaultOrientButton.Click += new System.EventHandler(this.DefaultOrientButton_Click);
            // 
            // QuatArrowHost
            // 
            this.QuatArrowHost.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.QuatArrowHost.Location = new System.Drawing.Point(249, 25);
            this.QuatArrowHost.Name = "QuatArrowHost";
            this.QuatArrowHost.Size = new System.Drawing.Size(506, 562);
            this.QuatArrowHost.TabIndex = 25;
            this.QuatArrowHost.Text = "elementHost2";
            this.QuatArrowHost.Child = null;
            // 
            // QuatBodyViewerHost
            // 
            this.QuatBodyViewerHost.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.QuatBodyViewerHost.Location = new System.Drawing.Point(249, 25);
            this.QuatBodyViewerHost.Name = "QuatBodyViewerHost";
            this.QuatBodyViewerHost.Size = new System.Drawing.Size(506, 562);
            this.QuatBodyViewerHost.TabIndex = 26;
            this.QuatBodyViewerHost.Text = "QuatBodyHost";
            this.QuatBodyViewerHost.Visible = false;
            this.QuatBodyViewerHost.Child = null;
            // 
            // BindToRightForearmButton
            // 
            this.BindToRightForearmButton.Location = new System.Drawing.Point(12, 550);
            this.BindToRightForearmButton.Name = "BindToRightForearmButton";
            this.BindToRightForearmButton.Size = new System.Drawing.Size(231, 23);
            this.BindToRightForearmButton.TabIndex = 28;
            this.BindToRightForearmButton.Text = "Set Default Orientation to Right Forearm";
            this.BindToRightForearmButton.UseVisualStyleBackColor = true;
            this.BindToRightForearmButton.Click += new System.EventHandler(this.BindToRightForearmButton_Click);
            // 
            // Charts
            // 
            this.Charts.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Charts.Location = new System.Drawing.Point(1037, 9);
            this.Charts.Name = "Charts";
            this.Charts.Size = new System.Drawing.Size(302, 758);
            this.Charts.TabIndex = 27;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1351, 793);
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
            this.Controls.Add(this.AccelChart);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.checkedListBox1);
            this.Name = "Form1";
            this.Text = "Bluetooth Sensor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Shown += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.AccelChart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.SensorDataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckedListBox checkedListBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataVisualization.Charting.Chart AccelChart;
        private System.Windows.Forms.FolderBrowserDialog FileLoggerFolderBrowser;
        private System.Windows.Forms.Button LoggingButton;
        private System.Windows.Forms.TextBox FileLocationTextBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button BrowseButton;
        private System.Windows.Forms.Integration.ElementHost elementHost1;
        private System.Windows.Forms.DataGridViewTextBoxColumn measurementsPerSecondDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridView SensorDataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn DeviceName;
        private System.Windows.Forms.DataGridViewComboBoxColumn SensorLocationCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn MPS;
        private System.Windows.Forms.DataGridViewTextBoxColumn DataPreview;
        private System.Windows.Forms.Integration.ElementHost SensorHost;
        private System.Windows.Forms.Button DefaultOrientButton;
        private System.Windows.Forms.Integration.ElementHost QuatArrowHost;
        private System.Windows.Forms.Integration.ElementHost QuatBodyViewerHost;
        private BluetoothController.SensorGrapher Charts;
        private System.Windows.Forms.Button BindToRightForearmButton;
    }
}

