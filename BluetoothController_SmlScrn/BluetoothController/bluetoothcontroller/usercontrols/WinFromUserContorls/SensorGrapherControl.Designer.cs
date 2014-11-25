namespace BluetoothController
{
    partial class SensorGrapherControl
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.SensorChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            ((System.ComponentModel.ISupportInitialize)(this.SensorChart)).BeginInit();
            this.SuspendLayout();
            // 
            // SensorChart
            // 
            chartArea1.Name = "ChartArea1";
            this.SensorChart.ChartAreas.Add(chartArea1);
            this.SensorChart.Dock = System.Windows.Forms.DockStyle.Fill;
            legend1.Name = "Legend1";
            this.SensorChart.Legends.Add(legend1);
            this.SensorChart.Location = new System.Drawing.Point(0, 0);
            this.SensorChart.Name = "SensorChart";
            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this.SensorChart.Series.Add(series1);
            this.SensorChart.Size = new System.Drawing.Size(423, 523);
            this.SensorChart.TabIndex = 0;
            this.SensorChart.Text = "SensorChart";
            // 
            // SensorGrapherControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.SensorChart);
            this.Name = "SensorGrapherControl";
            this.Size = new System.Drawing.Size(423, 523);
            ((System.ComponentModel.ISupportInitialize)(this.SensorChart)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart SensorChart;
    }
}
