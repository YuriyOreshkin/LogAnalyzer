using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace LogAnalyzer.Models.Domain.Entities
{
    public class LogString
    {
        public DateTime date { get; set; }
        public TimeSpan time { get; set; }
        public string serverIP { get; set; }
        public string method { get; set; }
        public string uri { get; set; }
        public string query { get; set; }
        public int port { get; set; }
        public string username { get; set; }
        public string clientIP { get; set; }
        public string agent { get; set; }
        public int status { get; set; }
    }
}
