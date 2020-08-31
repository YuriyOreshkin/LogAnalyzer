using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using LogAnalyzer.Models;
using LogAnalyzer.Models.Domain;
using Microsoft.AspNetCore.Mvc;

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
                    hasChildren=sites.GetSites().Any(c=>c.parentid == s.id)
                });
            return Json(apps.ToList());
        }
    }
}