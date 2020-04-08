using System.Collections.Generic;
using System.Linq;

using KenticoInspector.Core.Models;
using KenticoInspector.Core.Repositories;
using KenticoInspector.Core.Services;

namespace KenticoInspector.Infrastructure.Repositories
{
    public class SiteRepository : ISiteRepository
    {
        private readonly IDatabaseService databaseService;

        private const string getCmsSitesPath = @"Scripts/GetCmsSites.sql";

        public SiteRepository(IDatabaseService databaseService)
        {
            this.databaseService = databaseService;
        }

        public CmsSite GetSite(Instance instance, int siteID)
        {
            return GetSites(instance)
                .FirstOrDefault(site => site.SiteId == siteID);
        }

        public IEnumerable<CmsSite> GetSites(Instance instance)
        {
            var sites = databaseService.ExecuteSqlFromFile<CmsSite>(getCmsSitesPath);

            return sites;
        }
    }
}