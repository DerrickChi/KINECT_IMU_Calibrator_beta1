using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace BluetoothController
{
    public partial class SensorGrapherControl : UserControl
    {
        public static string LegendPostfix { get { return "_Leg"; } }
        public static string TitlePostfix { get { return "_Tit"; } }
        public static string[] SeriesXYZPostfix { get { return new string[] {"_X", "_Y", "_Z"}; } }
        public static Color[] DefaultXYZColor { get { return new Color[] {Color.MediumBlue, Color.Red, Color.DarkGreen}; } }
        public static string[] DefaultAclLegendText { get { return new string[] { "Acl X", "Acl Y", "Acl Z" }; } }
        public static string[] DefaultVelLegendText { get { return new string[] { "Vel X", "Vel Y", "Vel Z" }; } }
        public static string[] DefaultPosLegendText { get { return new string[] { "Pos X", "Pos Y", "Pos Z" }; } }
        public static string RefreshFreqPostFix { get { return "_RefreshFreq"; } }
        public static string RefreshCountPostFix { get { return "_RefreshCount"; } }

        public enum ChartTypes
        {
            Position,
            Velocity,
            Acceleration
        }

        public List<ChartInfo> ChartItemsList;
        private ConcurrentDictionary<string, int> RefreshDictionary = new ConcurrentDictionary<string,int>();
        public SensorGrapherControl()
        {
            InitializeComponent();
            this.ChartItemsList = new List<ChartInfo>();
            this.SensorChart.ChartAreas.Clear();
            this.SensorChart.Legends.Clear();
            this.SensorChart.Series.Clear();
            this.SensorChart.Titles.Clear();
        }

        public void UpdateGraph(ChartInfo chartinfo, double[] yvalues, long ticks)
        {
            if (this.RefreshDictionary[chartinfo.RefreshCounterName] >= this.RefreshDictionary[chartinfo.RefreshFrequencyName])
            {
                this.RefreshDictionary[chartinfo.RefreshCounterName] = 0;
                this.BeginInvoke((Action)delegate
                {
                    try
                    {
                        Chart c = this.SensorChart;
                        double min, max;
                        min = (new DateTime(ticks)).AddSeconds(-5).ToOADate();
                        max = (new DateTime(ticks)).ToOADate();
                        c.ChartAreas[chartinfo.ChartAreaName].AxisX.Minimum = min;
                        c.ChartAreas[chartinfo.ChartAreaName].AxisX.Maximum = max;
                        for (int i = 0; i < chartinfo.SeriesNames.Count; i++)
                        {
                            c.Series[chartinfo.SeriesNames[i]].Points.AddXY(max, yvalues[i]);
                            while (c.Series[chartinfo.SeriesNames[i]].Points[0].XValue < min)
                                c.Series[chartinfo.SeriesNames[i]].Points.RemoveAt(0);
                        }
                    }
                    catch (Exception)
                    {
                        System.Diagnostics.Debug.WriteLine("Exception throw on: BeginInoke on UpdateGraph in SensorGrapherControl");
                    }

                });
            }
            else
                this.RefreshDictionary[chartinfo.RefreshCounterName]++;
        }


        public ChartInfo SetupChartAreaFor(string rootname, ChartTypes charttype, int refreshfrequency)
        {
            Chart c = null;
            c = this.GetDefaultChartIfNull(c);
            ChartInfo chartinfo = this.AddChartAreaAndLegend(c, rootname);
            switch(charttype)
            {
                case ChartTypes.Acceleration:
                    this.ChartSeriesAndCustomization(c, chartinfo, SensorGrapherControl.SeriesXYZPostfix, SensorGrapherControl.DefaultXYZColor, SensorGrapherControl.DefaultAclLegendText, 45.0);
                    break;
                case ChartTypes.Velocity:
                    this.ChartSeriesAndCustomization(c, chartinfo, SensorGrapherControl.SeriesXYZPostfix, SensorGrapherControl.DefaultXYZColor, SensorGrapherControl.DefaultVelLegendText, 6.0);
                    break;
                case ChartTypes.Position:
                    this.ChartSeriesAndCustomization(c, chartinfo, SensorGrapherControl.SeriesXYZPostfix, SensorGrapherControl.DefaultXYZColor, SensorGrapherControl.DefaultPosLegendText, 4.0);
                    break;
            }
            this.InitilizeRefreshCountAndFreq(chartinfo, refreshfrequency);
            this.ChartItemsList.Add(chartinfo);
            return chartinfo;
 
        }

        public void RemoveChartViaChartInfo(ChartInfo chartinfo)
        {
            Chart c = null;
            c = this.GetDefaultChartIfNull(c);
            foreach (string s in chartinfo.SeriesNames)
                c.Series.Remove(c.Series[s]);
            c.Titles.Remove(c.Titles[chartinfo.TitleName]);
            c.Legends.Remove(c.Legends[chartinfo.LegendName]);
            c.ChartAreas.Remove(c.ChartAreas[chartinfo.ChartAreaName]);
        }


        private ChartInfo AddChartAreaAndLegend(Chart c, string rootname)
        {
            //c = this.GetDefaultChartIfNull(c);
            ChartInfo buildingchartinfo = new ChartInfo();
            buildingchartinfo.RootName = rootname;
            buildingchartinfo.ChartAreaName = rootname;
            c.ChartAreas.Add(new ChartArea() { Name = buildingchartinfo.ChartAreaName, AxisX = this.GetDefaultXAxis()});
           
            buildingchartinfo.LegendName = rootname + SensorGrapherControl.LegendPostfix;
            c.Legends.Add(new Legend { Name = buildingchartinfo.LegendName, TableStyle = LegendTableStyle.Wide, Alignment = StringAlignment.Far, BackColor = System.Drawing.SystemColors.InactiveBorder, DockedToChartArea = buildingchartinfo.RootName, Docking = Docking.Left });
            buildingchartinfo.TitleName = buildingchartinfo.RootName + SensorGrapherControl.TitlePostfix;
            c.Titles.Add(new Title { BackColor = System.Drawing.SystemColors.ActiveCaption, Text = buildingchartinfo.RootName, Name = buildingchartinfo.RootName + SensorGrapherControl.TitlePostfix, DockedToChartArea = buildingchartinfo.ChartAreaName, IsDockedInsideChartArea = true });
            return buildingchartinfo;

        }

        private Chart GetDefaultChartIfNull(Chart c)
        {
            return (c == null) ? this.SensorChart : c;
        }

        private Axis GetDefaultXAxis()
        {
            return new Axis() { Enabled = AxisEnabled.False };
        }

        private ChartInfo ChartSeriesAndCustomization(Chart c, ChartInfo info, string[] seriespostfixs, Color[] colors, string[] legendstexts, double axissize)
        {
            for(int i = 0; i < seriespostfixs.Length; i++)
            {
                info.SeriesNames.Add(info.RootName + seriespostfixs[i]);
                c.Series.Add(new Series() { Name = info.RootName + seriespostfixs[i], ChartArea = info.ChartAreaName, ChartType = SeriesChartType.FastLine, Color = colors[i], LegendText = legendstexts[i], Legend = info.LegendName });
            }
            c.ChartAreas[info.RootName].AxisY.Minimum = -axissize;
            c.ChartAreas[info.RootName].AxisY.Maximum = axissize;
            return info;           
        }

        private ChartInfo InitilizeRefreshCountAndFreq(ChartInfo chartinfo, int frequency)
        {
            chartinfo.RefreshCounterName = chartinfo.RootName + SensorGrapherControl.RefreshCountPostFix;
            chartinfo.RefreshFrequencyName = chartinfo.RootName + SensorGrapherControl.RefreshFreqPostFix;
            this.RefreshDictionary[chartinfo.RefreshCounterName] = 0;
            this.RefreshDictionary[chartinfo.RefreshFrequencyName] = frequency;
            return chartinfo;
        }
    }
}
