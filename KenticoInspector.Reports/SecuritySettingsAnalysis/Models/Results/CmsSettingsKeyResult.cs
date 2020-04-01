﻿using System;
using System.Collections.Generic;
using System.Linq;

using KenticoInspector.Core.Models;
using KenticoInspector.Reports.SecuritySettingsAnalysis.Models.Data;

namespace KenticoInspector.Reports.SecuritySettingsAnalysis.Models.Results
{
    public class CmsSettingsKeyResult
    {
        private readonly int siteID;
        private readonly string categoryIDPath;

        public string SiteName { get; set; }

        public int KeyID { get; set; }

        public string KeyPath { get; set; }

        public string KeyDisplayName { get; set; }

        public string KeyDefaultValue { get; set; }

        public string KeyValue { get; set; }

        public string RecommendedValue { get; set; }

        public string RecommendationReason { get; set; }

        public CmsSettingsKeyResult(CmsSettingsKey cmsSettingsKey, string recommendedValue, Term recommendationReason)
        {
            siteID = cmsSettingsKey.SiteID;
            categoryIDPath = cmsSettingsKey.CategoryIDPath;

            KeyID = cmsSettingsKey.KeyID;
            KeyDisplayName = cmsSettingsKey.KeyDisplayName;
            KeyDefaultValue = cmsSettingsKey.KeyDefaultValue;
            KeyValue = cmsSettingsKey.KeyValue;
            RecommendedValue = recommendedValue;
            RecommendationReason = recommendationReason;
        }

        public CmsSettingsKeyResult(
            CmsSettingsKeyResult cmsSettingsKeyResult,
            IEnumerable<CmsSettingsCategory> cmsSettingsCategories,
            IEnumerable<CmsSite> sites,
            IDictionary<string, string> resxValues
            )
        {
            SiteName = sites
                .FirstOrDefault(site => site.SiteId == cmsSettingsKeyResult.siteID)
                .SiteName;

            KeyID = cmsSettingsKeyResult.KeyID;

            var categoryDisplayNames = cmsSettingsKeyResult
                .GetCategoryIdsOnPath()
                .Select(idString => cmsSettingsCategories
                    .First(cmsSettingsCategory => cmsSettingsCategory
                        .CategoryID.ToString()
                        .Equals(idString))
                    .CategoryDisplayName)
                .Select(categoryDisplayName => TryReplaceDisplayName(resxValues, categoryDisplayName));

            KeyPath = string.Join(" > ", categoryDisplayNames);

            KeyDisplayName = TryReplaceDisplayName(
                resxValues,
                cmsSettingsKeyResult.KeyDisplayName
                );

            KeyDefaultValue = cmsSettingsKeyResult.KeyDefaultValue;
            KeyValue = cmsSettingsKeyResult.KeyValue;
            RecommendedValue = cmsSettingsKeyResult.RecommendedValue;
            RecommendationReason = cmsSettingsKeyResult.RecommendationReason;
        }

        public IEnumerable<string> GetCategoryIdsOnPath()
        {
            return categoryIDPath
                .Split('/', StringSplitOptions.RemoveEmptyEntries)
                .Select(pathSegment => pathSegment.TrimStart('0'));
        }

        private static string TryReplaceDisplayName(IDictionary<string, string> resxValues, string displayName)
        {
            displayName = displayName
                .Replace("{$", string.Empty)
                .Replace("$}", string.Empty)
                .ToLowerInvariant();

            if (resxValues.TryGetValue(displayName, out string keyName))
            {
                displayName = keyName;
            }

            return displayName;
        }
    }
}