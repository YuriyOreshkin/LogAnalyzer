using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LogAnalyzer.Models.Domain.Entities;

namespace LogAnalyzer.Models.Domain.Providers
{
    public class LogParseFiles : ILogParse
    {
        private ISitesProvider sitesprovider;
      

        public LogParseFiles(ISitesProvider _sitesprovider)
        {
            sitesprovider = _sitesprovider;
        }
        public IQueryable<LogString> GetLogStrings(Site site, DateTime datebedin,DateTime dateend)
        {
            List<LogString> log = new List<LogString>();
            //Fields order
            List<string> fields = new List<string>();

            string[] logsdirectories = string.IsNullOrEmpty(site.logpath) ? sitesprovider.GetSites().Where(p => p.parentid == site.id).Select(l => l.logpath).ToArray<string>() : new[] { site.logpath };

            foreach (string logsdirectory in logsdirectories)
            {

                if (Directory.Exists(logsdirectory))
                {

                    foreach (string filename in GetFilesNames(logsdirectory, datebedin, dateend))
                    {
                        try
                        {
                            IEnumerable<string> lines = File.ReadAllLines(filename);
                            foreach (string line in lines)
                            {
                                if (!line.StartsWith("#"))
                                {
                                    log.Add(LogStringParse(line, fields));
                                }
                                else if (line.StartsWith("#Fields:"))
                                {
                                    GetFields(line, ref fields);
                                }
                            }
                        }
                        catch (Exception ex) {
                          
                        }
                    }
                }
            }

         
            return FilterLog(site,log).AsQueryable<LogString>();
        }

        private void GetFields(string line, ref List<string> fields)
        {
            fields.Clear();

            string[] parsedstring = line.Substring(line.IndexOf(':')+2).Split(' ');

            fields = parsedstring.ToList();
        }

        private IEnumerable<LogString> FilterLog(Site site, List<LogString> log)
        {
            var result = log.AsEnumerable<LogString>();

            var level = site.id.Where(s => s == '.').Count();

            if (level== 0 )
            {   //Site
                result = result.Where(s => s.port == site.port);
            }

            //App
            if (level == 1)
            {
                result = result.Where(u => u.uri.StartsWith(site.name + "/"));
            }

            return result;
        }

        private LogString LogStringParse(string line,List<string> fields)
        {
            string[] parsedstring = line.Split(' ');

            return new LogString
            {
                date = GetFieldIndex(fields, "date") >-1 ? DateTime.Parse(parsedstring[GetFieldIndex(fields, "date")]) : DateTime.MinValue,
                time = GetFieldIndex(fields, "time") > -1 ? TimeSpan.Parse(parsedstring[GetFieldIndex(fields, "time")]) : TimeSpan.MinValue,
                serverIP = GetFieldIndex(fields, "s-ip") > -1 ? parsedstring[GetFieldIndex(fields, "s-ip")] : "",
                method = GetFieldIndex(fields, "cs-method") > -1 ? parsedstring[GetFieldIndex(fields, "cs-method")] : "",
                uri = GetFieldIndex(fields, "cs-uri-stem") > -1 ? parsedstring[GetFieldIndex(fields, "cs-uri-stem")] : "",
                query = GetFieldIndex(fields, "cs-uri-query") > -1 ? parsedstring[GetFieldIndex(fields, "cs-uri-query")] : "",
                port = GetFieldIndex(fields, "s-port") > -1 ? int.Parse(parsedstring[GetFieldIndex(fields, "s-port")]) : 0,
                username = GetFieldIndex(fields, "cs-username") > -1 ? parsedstring[GetFieldIndex(fields, "cs-username")] : "",
                clientIP = GetFieldIndex(fields, "c-ip") > -1 ? parsedstring[GetFieldIndex(fields, "c-ip")] : "",
                agent = GetFieldIndex(fields, "cs(User-Agent)") > -1 ? parsedstring[GetFieldIndex(fields, "cs(User-Agent)")].Replace('+', ' ') : "",
                status = GetFieldIndex(fields, "sc-status") > -1 ? int.Parse(parsedstring[GetFieldIndex(fields, "sc-status")]) : 0
            };
        }

        private int GetFieldIndex(List<string> fields, string fieldname)
        {
            return fields.FindIndex(l => l == fieldname);
        }

        private IEnumerable<string> GetFilesNames(string directory,DateTime datebedin,DateTime dateend)
        {
            try
            {
                return Directory.GetFiles(directory, "*.log", SearchOption.AllDirectories).Where(f => InPeriod(f, datebedin, dateend));
            }
            catch {

                return Enumerable.Empty<string>();
            }
        }

        private bool InPeriod(string fullfilename, DateTime datebedin, DateTime dateend)
        {
            string filename = Path.GetFileNameWithoutExtension(fullfilename);
            string filedate =filename.Substring(4,6);
            DateTime datefile = DateTime.ParseExact(filedate, "yyMMdd", null);
            var begin = new DateTime(datebedin.Year,datebedin.Month,datebedin.Day);
            var end = new DateTime(dateend.Year, dateend.Month, dateend.Day);

            if (datefile >= begin && datefile <= end)
                return true;

            return false;

        }  


    }
}
