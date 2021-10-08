using LogAnalyzer.Models.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogAnalyzer.Models.Domain
{
    public interface ISitesProvider
    {
        IQueryable<Site> GetSites();
    }
}
