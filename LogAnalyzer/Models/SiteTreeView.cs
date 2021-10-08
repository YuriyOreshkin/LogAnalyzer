using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace LogAnalyzer.Models
{
    public class SiteTreeView
    {
        public string id { get; set; }
        public string name { get; set; }
        public bool hasChildren { get; set; }
    }
}
