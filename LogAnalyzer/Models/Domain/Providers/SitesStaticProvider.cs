using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LogAnalyzer.Models.Domain.Entities;

namespace LogAnalyzer.Models.Domain.Providers
{
    public class SitesStaticProvider : ISitesProvider
    {
        public IQueryable<SiteTree> GetSites()
        {
            List<SiteTree> sites = new List<SiteTree>()
            {
                new SiteTree { id = 1, name = "Default", logpath = @"C:\Users\007OreshkinYV\source\repos\LogAnalyzer\LogAnalyzer\App_Data\", parentid = null },
                new SiteTree { id = 2, name = "IAP", logpath = @"C:\Users\007OreshkinYV\source\repos\LogAnalyzer\LogAnalyzer\App_Data\", parentid = 1 },
                new SiteTree { id = 3, name = "aps", logpath = @"C:\Users\007OreshkinYV\source\repos\LogAnalyzer\LogAnalyzer\App_Data\", parentid = 1 },
                new SiteTree { id = 4, name = "APS2", logpath = @"C:\Users\007OreshkinYV\source\repos\LogAnalyzer\LogAnalyzer\App_Data\", parentid = 1 }
            };

            return sites.AsQueryable<SiteTree>();
        }
    }
}
