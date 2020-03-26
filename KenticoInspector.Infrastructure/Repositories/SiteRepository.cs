﻿using System.Collections.Generic;
using System.Linq;

using KenticoInspector.Core.Models;
using KenticoInspector.Core.Repositories.Interfaces;
using KenticoInspector.Core.Services.Interfaces;

namespace KenticoInspector.Infrastructure.Repositories
{
    public class SiteRepository : ISiteRepository
    {
        private readonly IDatabaseService databaseService;

        private static readonly string getCmsSitesPath = @"Scripts/GetCmsSites.sql";

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