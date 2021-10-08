using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogAnalyzer.Models
{
    public class ChartSitesHitsView
    {
        public string SiteName { get; set; }
        public int Hits { get; set; }
        public DateTime Date { get; set; }
    }
}
