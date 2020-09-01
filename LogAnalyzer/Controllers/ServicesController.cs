using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using LogAnalyzer.Models;
using LogAnalyzer.Models.Domain;
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
        public IActionResult GridRead([DataSourceRequest]DataSourceRequest request, DateTime datebegin,DateTime dateend)
        {
            return Json(log.GetLogStrings().ToDataSourceResult(request));
        }

        public IActionResult TreeRead(long? id)
        {
            var apps =sites.GetSites().Where(e => id.HasValue ? e.parentid == id : e.parentid == null)
                .Select(s=> new {
                    id=s.id,
                    name=s.name,
                    issite = s.issite,
                    hasChildren=sites.GetSites().Any(c=>c.parentid == s.id)
                });
            return Json(apps.ToList());
        }

        [HttpPost]
        public IActionResult ChartSitesHits([Bind(Prefix = "sites")]IEnumerable<long> sites, DateTime datebegin, DateTime dateend)
        {
            IEnumerable<ChartSitesHitsView> result = 
            log.GetLogStrings().Where(uri=>uri.uri.Contains("APS")).ToList().Select(s =>
                new {

                    SiteName = s.uri.Split('/')[1],
                    Date = s.date

                }).GroupBy(g => new { g.SiteName, g.Date }).Select(r =>

                  new ChartSitesHitsView
                  {
                      SiteName = r.Key.SiteName,
                      Hits = r.Count(),
                      Date = r.Key.Date
                  }
            );

            return Json(result);
        }

        [HttpPost]
        public IActionResult ChartUniqVisitors([Bind(Prefix = "sites")]IEnumerable<long> sites, DateTime datebegin, DateTime dateend)
        {
            IEnumerable<ChartSitesUniqVisitorsView> result = 
            log.GetLogStrings().Where(w => w.uri.Contains("APS")).ToList().Select(s=>

             new {

                 SiteName =  s.uri.Split('/')[1],
                 Visitor = s.clientIP,
                 Date = s.date
             })
             .GroupBy(g=>new { g.SiteName, g.Date, g.Visitor }).Select(r=> 
            
                new ChartSitesUniqVisitorsView
                {
                    SiteName = r.Key.SiteName,
                    UniqVisitors = 1,
                    Date = r.Key.Date
                }
            );

            return Json(result);
        }

        [HttpPost]
        public IActionResult ChartHTTPStatus([Bind(Prefix = "sites")]IEnumerable<long> sites, DateTime datebegin, DateTime dateend)
        {
            var logstrings = log.GetLogStrings().Where(w => w.uri.Contains("APS")).ToList();

            IEnumerable <ChartHTTPStatusView> result =
                logstrings.Select(s =>
             new {
                
                 Status = s.status
             })
             .GroupBy(g => g.Status).Select(r =>

                  new ChartHTTPStatusView
                  {
                      Status = r.Key.ToString(),
                      Percentage= r.Count() * 100 / logstrings.Count()
                  }
            );
            return Json(result);
        }

        [HttpPost]
        public IActionResult ChartBrowsers([Bind(Prefix = "sites")]IEnumerable<long> sites, DateTime datebegin, DateTime dateend)
        {
            var logstrings = log.GetLogStrings().Where(w => w.uri.Contains("APS")).ToList();

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


        private string BrowserName(string agent)
        {
            var uaParser = Parser.GetDefault();
            UserAgent client = uaParser.ParseUserAgent(agent);

            return client.Family;
        }
    }
}