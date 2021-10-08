using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using LogAnalyzer.Models;
using LogAnalyzer.Models.Domain;
using LogAnalyzer.Models.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using UAParser;

namespace LogAnalyzer.Controllers
{
    public class ServicesController : Controller
    {
        private ILogParse log;
        private ISitesProvider sites;
        public ServicesController(ILogParse _log,ISitesProvider _sites)
        {
            log = _log;
            sites = _sites;
        }

        public IActionResult GridRead([DataSourceRequest]DataSourceRequest request, [Bind(Prefix = "sites")]IEnumerable<string> sites, DateTime datebegin,DateTime dateend)
        {
            var result = GetSitesLog(sites, datebegin, dateend).OrderByDescending(o => o.date).ThenByDescending(o => o.time);

            
            return Json(result.ToDataSourceResult(request));
        }

      

        public IActionResult TreeRead(string id)
        {

            var ss = sites.GetSites();

            var apps =sites.GetSites().Where(e => string.IsNullOrEmpty(id) ?  string.IsNullOrEmpty(e.parentid) : e.parentid == id ).ToList().Select(s=>ConvertFromModel(s));

            return Json(apps);
        }

        [HttpPost]
        public IActionResult ChartSitesHits([Bind(Prefix = "sites")]IEnumerable<string> sites, DateTime datebegin, DateTime dateend)
        {
            IEnumerable<ChartSitesHitsView> result = GetSitesLog(sites,datebegin,dateend)
                                                        .GroupBy(g => new { g.sitename, g.date }).Select(r =>

                                                              new ChartSitesHitsView
                                                              {
                                                                  SiteName = r.Key.sitename,
                                                                  Hits = r.Count(),
                                                                  Date = r.Key.date
                                                              }
                                                         );

            return Json(result);
        }

        [HttpPost]
        public IActionResult ChartUniqVisitors([Bind(Prefix = "sites")]IEnumerable<string> sites, DateTime datebegin, DateTime dateend)
        {
            IEnumerable<ChartSitesUniqVisitorsView> result = GetSitesLog(sites, datebegin, dateend)
             .GroupBy(g=>new { g.sitename, g.date, g.clientIP }).Select(r=> 
            
                new ChartSitesUniqVisitorsView
                {
                    SiteName = r.Key.sitename,
                    UniqVisitors = 1,
                    Date = r.Key.date
                }
            );

            return Json(result);
        }


        [HttpPost]
        public IActionResult ChartHTTPStatus([Bind(Prefix = "sites")]IEnumerable<string> sites, DateTime datebegin, DateTime dateend)
        {
            var logstrings = GetSitesLog(sites, datebegin, dateend).ToList();

            IEnumerable <ChartHTTPStatusView> result = logstrings
                                                             .GroupBy(g => g.status).Select(r =>

                                                                  new ChartHTTPStatusView
                                                                  {
                                                                      Status = r.Key.ToString(),
                                                                      Percentage= r.Count() * 100 / logstrings.Count()
                                                                  }
                                                            );
            return Json(result);
        }

        [HttpPost]
        public IActionResult ChartBrowsers([Bind(Prefix = "sites")]IEnumerable<string> sites, DateTime datebegin, DateTime dateend)
        {
            var logstrings = GetSitesLog(sites, datebegin, dateend).ToList();

            IEnumerable<ChartBrowsersView> result =
                            logstrings.GroupBy(g => g.agent).Select(s =>
                             new {

                                 Browser = BrowserName(s.Key),
                                 Percentage =s.Count() 
                             }).
                             ToList().GroupBy(g => g.Browser).Select(r =>

                                  new ChartBrowsersView
                                  {
                                      Browser =  r.Key,
                                      Percentage = r.Sum(c=>c.Percentage)*100 / logstrings.Count()
                                  }
                            );
            return Json(result);
        }

        [HttpPost]
        public IActionResult ChartClientOS([Bind(Prefix = "sites")]IEnumerable<string> sites, DateTime datebegin, DateTime dateend)
        {
            var logstrings = GetSitesLog(sites, datebegin, dateend).ToList();

            IEnumerable<ChartBrowsersView> result =
                                    logstrings.GroupBy(g => g.agent).Select(s =>
                                     new {

                                         Browser = OSName(s.Key),
                                         Percentage = s.Count()
                                     }).
                                     ToList().GroupBy(g => g.Browser).Select(r =>

                                          new ChartBrowsersView
                                          {
                                              Browser = r.Key,
                                              Percentage = r.Sum(c => c.Percentage) * 100 / logstrings.Count()
                                          }
                                    );
            return Json(result);
        }

        private IEnumerable<GridRowLogView> GetSitesLog(IEnumerable<string> sitesid, DateTime datebegin, DateTime dateend)
        {
            var result = Enumerable.Empty<GridRowLogView>();

            foreach (Site site in sites.GetSites().Where(s => sitesid.Contains(s.id)).ToList())
            {
                result = result.Concat(GetSiteLog(site,datebegin,dateend));      
            }

            return result;
        }


        private SiteTreeView ConvertFromModel(Site site)
        {
            return new SiteTreeView
            {
                id = site.id,
                name = site.name,
                hasChildren = sites.GetSites().Any(c => c.parentid == site.id)
            };
        }

        private IEnumerable<GridRowLogView> GetSiteLog(Site site, DateTime datebegin, DateTime dateend)
        {

            var result = log.GetLogStrings(site,datebegin,dateend).ToList().Select(s =>
            new GridRowLogView
            {
                date = s.date,
                time= s.time,
                uri = s.uri +System.Net.WebUtility.UrlDecode(s.query == "-" ? "" : s.query),
                method = s.method,
                sitename =site.name,
                clientIP = s.clientIP,
                username = s.username,
                agent =s.agent,
                status =s.status
            });

            return result;
        }

        private string BrowserName(string agent)
        {
            var uaParser = Parser.GetDefault();
            UserAgent client = uaParser.ParseUserAgent(agent);

            return client.Family;
        }

        private string OSName(string agent)
        {
            var uaParser = Parser.GetDefault();
            OS client = uaParser.ParseOS(agent);

            return client.Family;
        }
    }
}