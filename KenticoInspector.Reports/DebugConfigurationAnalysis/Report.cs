using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using KenticoInspector.Core;
using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Helpers;
using KenticoInspector.Core.Models;
using KenticoInspector.Core.Models.Results;
using KenticoInspector.Core.Services.Interfaces;
using KenticoInspector.Reports.DebugConfigurationAnalysis.Models;
using KenticoInspector.Reports.DebugConfigurationAnalysis.Models.Data;

namespace KenticoInspector.Reports.DebugConfigurationAnalysis
{
    public class Report : AbstractReport<Terms>
    {
        private readonly IDatabaseService _databaseService;
        private readonly IInstanceService _instanceService;
        private readonly ICmsFileService _cmsFileService;

        public Report(
            IDatabaseService databaseService,
            IInstanceService instanceService,
            ICmsFileService cmsFileService,
            IReportMetadataService reportMetadataService
        ) : base(reportMetadataService)
        {
            _databaseService = databaseService;
            _instanceService = instanceService;
            _cmsFileService = cmsFileService;
        }

        public override IList<Version> CompatibleVersions => VersionHelper.GetVersionList("10", "11", "12");

        public override IList<string> Tags => new List<string>
        {
           ReportTags.Health
        };

        public override ReportResults GetResults()
        {
            var instance = _instanceService.CurrentInstance;

            var databaseSettingsValues = _databaseService.ExecuteSqlFromFile<CmsSettingsKey>(Scripts.GetCMSSettingsKeysForDebug);

            var resxValues = _cmsFileService.GetResourceStringsFromResx(instance.Path);

            ResolveSettingsDisplayNames(databaseSettingsValues, resxValues);

            var webConfig = _cmsFileService.GetXDocument(instance.Path, DefaultKenticoPaths.WebConfigFile);

            var compilationDebugIsEnabled = GetBooleanValueofSectionAttribute(webConfig, "system.web", "compilation", "debug");
            var traceIsEnabled = GetBooleanValueofSectionAttribute(webConfig, "system.web", "trace", "enabled");

            return CompileResults(databaseSettingsValues, compilationDebugIsEnabled, traceIsEnabled);
        }

        private static void ResolveSettingsDisplayNames(IEnumerable<CmsSettingsKey> databaseSettingsValues, IDictionary<string, string> resxValues)
        {
            foreach (var databaseSettingsValue in databaseSettingsValues)
            {
                var key = databaseSettingsValue.KeyDisplayName
                    .Replace("{$", string.Empty)
                    .Replace("$}", string.Empty)
                    .ToLowerInvariant();

                if (resxValues.TryGetValue(key, out string value))
                {
                    databaseSettingsValue.KeyDisplayName = value;
                }
            }
        }

        private static bool GetBooleanValueofSectionAttribute(XDocument webConfig, string subSection, string element, string attributeName)
        {
            var valueRaw = webConfig
                .Descendants(subSection)
                .FirstOrDefault()
                .Element(element)?
                .Attribute(attributeName)?
                .Value;

            bool.TryParse(valueRaw, out bool value);

            return value;
        }

        private ReportResults CompileResults(IEnumerable<CmsSettingsKey> databaseSettingsKeys, bool compilationDebugIsEnabled, bool traceIsEnabled)
        {
            var explicitlyEnabledSettings = databaseSettingsKeys
                .Where(databaseSettingsKey => databaseSettingsKey.KeyValue == true && databaseSettingsKey.KeyDefaultValue == false);

            var compilationDebugAndTraceAreEnabled = compilationDebugIsEnabled || traceIsEnabled;

            if (!explicitlyEnabledSettings.Any() && !compilationDebugAndTraceAreEnabled)
            {
                return new ReportResults(ReportResultsStatus.Good)
                {
                    Summary = Metadata.Terms.GoodSummary
                };
            }

            var results = new ReportResults(ReportResultsStatus.Warning);

            if (explicitlyEnabledSettings.Any())
            {
                var explicitlyEnabledSettingsCount = explicitlyEnabledSettings.Count();

                results.Summary += Metadata.Terms.WarningSummary.With(new { explicitlyEnabledSettingsCount });
                results.Data.Add(explicitlyEnabledSettings.AsResult().WithLabel(Metadata.Terms.TableNames.ExplicitlyEnabledSettings));
            }

            results.Data.Add(databaseSettingsKeys.AsResult().WithLabel(Metadata.Terms.TableNames.Overview));

            if (compilationDebugAndTraceAreEnabled)
            {
                results.Status = ReportResultsStatus.Error;
                results.Summary += Metadata.Terms.ErrorSummary.With(new { compilationDebugIsEnabled, traceIsEnabled });
            }

            var webConfigSettingsKeys = new List<CmsSettingsKey>
            {
                new CmsSettingsKey("Debug", Metadata.Terms.WebConfig.DebugKeyDisplayName, compilationDebugIsEnabled, false),
                new CmsSettingsKey("Trace", Metadata.Terms.WebConfig.TraceKeyDisplayName, traceIsEnabled, false)
            };

            results.Data.Add(webConfigSettingsKeys.AsResult().WithLabel(Metadata.Terms.TableNames.WebConfig));

            return results;
        }
    }
}