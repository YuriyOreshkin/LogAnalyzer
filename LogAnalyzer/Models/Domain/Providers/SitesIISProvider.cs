using LogAnalyzer.Models.Domain.Entities;
using Microsoft.Web.Administration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogAnalyzer.Models.Domain.Providers
{
    public class SitesIISProvider
    {
        public IQueryable<SiteTree> GetSites()
        {
            List<SiteTree> sites = new List<SiteTree>();

            var iisManager = ServerManager.OpenRemote("10.7.99.191");
            foreach (Site site in iisManager.Sites)
            {

                sites.Add(
                    new SiteTree
                    {
                        id = site.Id,
                        name = site.Name,
                        logpath = site.LogFile.Directory,
                        parentid = null
                    });

                /*foreach (Application app in site.Applications)
                {
                    Console.WriteLine(
                        "{0} is assigned to the '{1}' application pool.",
                        app.Path, app.ApplicationPoolName);
                }*/
            }
            return sites.AsQueryable<SiteTree>();
        }
    }
}
