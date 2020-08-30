using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using LogAnalyzer.Models;
using Microsoft.AspNetCore.Mvc;

namespace LogAnalyzer.Controllers
{
    public class ServicesController : Controller
    {
        private ILogParse log;
        public ServicesController(ILogParse _log)
        {
            log = _log;
        }
        public IActionResult GridRead([DataSourceRequest]DataSourceRequest request)
        {
            return Json(log.GetLogStrings().ToDataSourceResult(request));
        }
    }
}