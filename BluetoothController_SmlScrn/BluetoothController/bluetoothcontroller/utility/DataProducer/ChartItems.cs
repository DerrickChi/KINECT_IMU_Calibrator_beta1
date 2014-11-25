using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothController
{
    public class ChartInfo
    {
        public string RootName {get; set;}
        public string ChartAreaName { get; set; }
        public string LegendName { get; set; }
        public string TitleName { get; set; }
        public List<string> SeriesNames { get; set; }
        public string RefreshCounterName { get; set; }
        public string RefreshFrequencyName { get; set; }



        public ChartInfo() { this.SeriesNames = new List<string>(); }


        public ChartInfo(string rootname, string chartareaname, string legendname, string title, IEnumerable<string> seriesnames)
        {
            this.RootName = rootname;
            this.ChartAreaName = chartareaname;
            this.LegendName = legendname;
            this.TitleName = title;
            this.SeriesNames = new List<string>(seriesnames);
        }

        public static ChartInfo operator+ (ChartInfo l, ChartInfo r)
        {
            ChartInfo c = new ChartInfo();
            if (r.RootName != null)
                c.RootName = r.RootName;
            else
                c.RootName = l.RootName;

            if (r.ChartAreaName != null)
                c.ChartAreaName = r.ChartAreaName;
            else
                c.ChartAreaName = l.ChartAreaName;

            if (r.LegendName != null)
                c.LegendName = r.LegendName;
            else
                c.LegendName = l.LegendName;

            if (r.TitleName != null)
                c.TitleName = r.TitleName;
            else
                c.TitleName = l.TitleName;

            c.SeriesNames.AddRange(l.SeriesNames);
            c.SeriesNames.AddRange(r.SeriesNames);
            return c;
        }
    }

    public static class ChartItemHelpers
    {
        public static List<ChartInfo> UpdateOrAdd(this List<ChartInfo> l, ChartInfo chartitemtoadd)
        {
            ChartInfo tci;
            if ((tci = l.FirstOrDefault(x => x.RootName == chartitemtoadd.RootName)) != null)
                tci += chartitemtoadd;
            else
                l.Add(chartitemtoadd);
            return l;
        }
        public static ChartInfo AsCopy(this ChartInfo c)
        {
            return new ChartInfo() { ChartAreaName = c.ChartAreaName, RootName = c.RootName, TitleName = c.TitleName, LegendName = c.LegendName, RefreshCounterName = c.RefreshCounterName, RefreshFrequencyName = c.RefreshFrequencyName, SeriesNames = new List<string>(c.SeriesNames.ToArray())  };
        }
    }
}
