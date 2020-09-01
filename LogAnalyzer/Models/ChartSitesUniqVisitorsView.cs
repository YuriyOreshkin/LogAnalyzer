using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogAnalyzer.Models
{
    public class ChartSitesUniqVisitorsView
    {
        public string SiteName { get; set; }
        public int UniqVisitors { get; set; }
        public DateTime Date { get; set; }
    }
}
