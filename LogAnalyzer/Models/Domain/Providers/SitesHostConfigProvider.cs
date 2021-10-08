using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using LogAnalyzer.Models.Domain.Entities;
using Microsoft.Extensions.Configuration;

namespace LogAnalyzer.Models.Domain.Providers
{
    public class SitesHostConfigProvider : ISitesProvider
    {
        private readonly IConfiguration configuration;

        public SitesHostConfigProvider(IConfiguration _configuration)
        {
            configuration = _configuration;
        }

        public IQueryable<Site> GetSites()
        {
            var result = new List<Site>();
            var config = configuration["IISConfigFile"];
           if (File.Exists(config))
                GetSitesFromServer(config,ref result);
            

            return result.AsQueryable();
        }

        private void GetSitesFromServer(string config, ref List<Site> result)
        {

            XDocument xRoot = XDocument.Load(config);
            XElement node = xRoot.Root;
            XElement sites = node.Descendants().FirstOrDefault(x => x.Name.LocalName == "sites");
            if (sites != null)
            {
                var defaultLog = sites.Element("siteDefaults").Element("logFile").Attribute("directory").Value.ToString();
                foreach (XElement siteElement in sites.Elements().Where(n => n.Name.LocalName == "site"))
                {

                    var site = SiteToSite(siteElement, defaultLog);
                    result.Add(site);
                    int i = 0;

                    foreach (XElement application in siteElement.Elements().Where(n => n.Name.LocalName == "application"))
                    {
                        i++;
                        var app = ApplicationToSite(application, i, site);
                        if(app.name !="/")
                               result.Add(app);
                    }

                }
            }

        }


        


        private Site SiteToSite(XElement site,string defaultLog)
        {
            return new Site
            {
                id = site.Attribute("id").Value.ToString(),
                parentid = null,
                name = site.Attribute("name").Value.ToString(),
                port = int.Parse(new Regex(@"\d+").Match(site.Element("bindings").Elements().FirstOrDefault(x => x.Attribute("protocol").Value == "http").Attribute("bindingInformation").Value).Value),
                logpath =site.Element("logFile") != null ? site.Element("logFile").Attribute("directory").Value.ToString() : defaultLog
                
            };
        }

        private Site ApplicationToSite(XElement application,int count, Site site)
        {
            return new Site
            {
                id = site.id + "." + count.ToString(),
                parentid = site.id,
                name = application.Attribute("path").Value.ToString(),
                port = site.port,
                logpath = site.logpath

            };
        }

    }
}
