using System;
using System.Collections.Generic;
using System.Linq;

using KenticoInspector.Core.Instances.Models;
using KenticoInspector.Reports.SecuritySettingsAnalysis.Models.Data;

using Newtonsoft.Json;

namespace KenticoInspector.Reports.SecuritySettingsAnalysis.Models.Results
{
    public class CmsSettingsKeyResult
    {
        private readonly string? categoryIdPath;

        [JsonIgnore]
        public int SiteID { get; set; }

        public string SiteName { get; set; } = null!;

        public int KeyID { get; set; }

        public string KeyPath { get; set; } = null!;

        public string KeyDisplayName { get; set; } = null!;

        public string? KeyDefaultValue { get; set; }

        public string? KeyValue { get; set; }

        public string RecommendedValue { get; set; } = null!;

        public string RecommendationReason { get; set; } = null!;

        public CmsSettingsKeyResult(string? categoryIdPath)
        {
            this.categoryIdPath = categoryIdPath;
        }

        public CmsSettingsKeyResult(
            CmsSettingsKeyResult cmsSettingsKeyResult,
            IEnumerable<CmsSettingsCategory> cmsSettingsCategories,
            IEnumerable<CmsSite> sites,
            IDictionary<string, string> resxValues
            )
        {
            SiteName = sites
                .FirstOrDefault(site => site.SiteId == cmsSettingsKeyResult.SiteID)
                .SiteName;

            KeyID = cmsSettingsKeyResult.KeyID;

            var categoryDisplayNames = cmsSettingsKeyResult
                .GetCategoryIdsOnPath()
                .Select(
                    idString => cmsSettingsCategories
                        .First(
                            cmsSettingsCategory => cmsSettingsCategory
                                .CategoryID.ToString()
                                .Equals(idString)
                            )
                        .CategoryDisplayName
                    )
                .Select(
                    categoryDisplayName => TryReplaceDisplayName(
                        resxValues,
                        categoryDisplayName
                        )
                    );

            KeyPath = string.Join(
                " > ",
                categoryDisplayNames
                );

            KeyDisplayName = TryReplaceDisplayName(
                resxValues,
                cmsSettingsKeyResult.KeyDisplayName
                );

            KeyDefaultValue = cmsSettingsKeyResult.KeyDefaultValue;
            KeyValue = cmsSettingsKeyResult.KeyValue;
            RecommendedValue = cmsSettingsKeyResult.RecommendedValue;
            RecommendationReason = cmsSettingsKeyResult.RecommendationReason;
        }

        public IEnumerable<string>? GetCategoryIdsOnPath()
        {
            return categoryIdPath?
                .Split(
                    '/',
                    StringSplitOptions.RemoveEmptyEntries
                    )
                .Select(pathSegment => pathSegment.TrimStart('0'));
        }

        private static string TryReplaceDisplayName(
            IDictionary<string, string> resxValues,
            string displayName
            )
        {
            displayName = displayName
                .Replace(
                    "{$",
                    string.Empty
                    )
                .Replace(
                    "$}",
                    string.Empty
                    )
                .ToLowerInvariant();

            if (resxValues.TryGetValue(
                displayName,
                out var keyName
                )) displayName = keyName;

            return displayName;
        }
    }
}