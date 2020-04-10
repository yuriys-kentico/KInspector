﻿using System.Collections.Generic;
using System.Linq;

using KenticoInspector.Core.Instances.Models;
using KenticoInspector.Core.Instances.Repositories;
using KenticoInspector.Core.Instances.Services;

namespace KenticoInspector.Instances.Repositories
{
    public class SiteRepository : ISiteRepository
    {
        private readonly IDatabaseService databaseService;

        private const string getCmsSitesPath = @"Scripts/GetCmsSites.sql";

        public SiteRepository(IDatabaseService databaseService)
        {
            this.databaseService = databaseService;
        }

        public CmsSite GetSite(Instance instance, int siteID) => GetSites(instance)
                .FirstOrDefault(site => site.SiteId == siteID);

        public IEnumerable<CmsSite> GetSites(Instance instance) => databaseService.ExecuteSqlFromFile<CmsSite>(getCmsSitesPath);
    }
}