using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogAnalyzer.Models.Domain.Entities
{
    public class Site
    {
       public string id { get; set; }
       public string name { get; set; }
       public string logpath { get; set; }
       public int port { get; set; }  
       public string parentid { get; set; }
    }
}
