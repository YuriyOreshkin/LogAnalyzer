using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace LogAnalyzer.Models
{
    public class GridRowLogView
    {
        [DisplayName("Date")]
        public DateTime date { get; set; }
        [DisplayName("Time")]
        public TimeSpan time { get; set; }
        [DisplayName("Site")]
        public string sitename { get; set; }
        [DisplayName("Method")]
        public string method { get; set; }
        [DisplayName("request URI")]
        public string uri { get; set; }
        [DisplayName("User")]
        public string username { get; set; }
        [DisplayName("IP client")]
        public string clientIP { get; set; }
        [DisplayName("Client")]
        public string agent { get; set; }
        [DisplayName("Status")]
        public int status { get; set; }
    }
}
