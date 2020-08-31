using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Web.Administration;
using System.Threading.Tasks;
using LogAnalyzer.Models.Domain.Entities;

namespace LogAnalyzer.Models.Domain.Providers
{
    public class LogParseFiles : ILogParse
    {
        readonly string logsdirectory= @"C:\Users\007OreshkinYV\source\repos\LogAnalyzer\LogAnalyzer\App_Data\";
        public IQueryable<LogString> GetLogStrings()
        {
            List<LogString> log = new List<LogString>();
            foreach (string filename in GetFilesNames(logsdirectory))
            {
                IEnumerable<string> lines = File.ReadAllLines(filename).Where(l=>!l.StartsWith('#'));
                foreach (string line in lines)
                {
                    log.Add(LogStringParse(line));
                }
            }

            return log.AsQueryable<LogString>();
        }

        private LogString LogStringParse(string line)
        {
            string[] parsedstring = line.Split(' ');

            return new LogString
            {
                date = DateTime.Parse(parsedstring[0]),
                time = TimeSpan.Parse(parsedstring[1]),
                serverIP = parsedstring[2],
                method = parsedstring[3],
                uri = parsedstring[4],
                query = parsedstring[5],
                port = int.Parse(parsedstring[6]),
                username = parsedstring[7],
                clientIP = parsedstring[8],
                agent = parsedstring[9].Replace('+', ' '),
                status = int.Parse(parsedstring[10])
            };
        }

        private IEnumerable<string> GetFilesNames(string directory)
        {
            return Directory.GetFiles(directory,"*", SearchOption.AllDirectories);
        }

    
    }
}
