using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using KenticoInspector.Core;
using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Models.Results;
using KenticoInspector.Core.Services;
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
            ICmsFileService cmsFileService
        )
        {
            _databaseService = databaseService;
            _instanceService = instanceService;
            _cmsFileService = cmsFileService;
        }

        public override IList<string> Tags => new List<string>
        {
           ReportTags.Health
        };

        [SupportsVersions("10 - 12.0")]
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

                if (resxValues.TryGetValue(key, out string? value))
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
                return new ReportResults(ResultsStatus.Good)
                {
                    Summary = Metadata.Terms.GoodSummary
                };
            }

            var results = new ReportResults(ResultsStatus.Warning);

            if (explicitlyEnabledSettings.Any())
            {
                var explicitlyEnabledSettingsCount = explicitlyEnabledSettings.Count();

                results.Summary += Metadata.Terms.WarningSummary.With(new { explicitlyEnabledSettingsCount });
                results.Data.Add(explicitlyEnabledSettings.AsResult().WithLabel(Metadata.Terms.TableNames.ExplicitlyEnabledSettings));
            }

            results.Data.Add(databaseSettingsKeys.AsResult().WithLabel(Metadata.Terms.TableNames.Overview));

            if (compilationDebugAndTraceAreEnabled)
            {
                results.Status = ResultsStatus.Error;
                results.Summary += Metadata.Terms.ErrorSummary.With(new { compilationDebugIsEnabled, traceIsEnabled });
            }

            var webConfigSettingsKeys = new List<WebConfigSettingsKey>
            {
                new WebConfigSettingsKey
                {
                    KeyName = "Debug",
                    KeyDisplayName = Metadata.Terms.WebConfig.DebugKeyDisplayName,
                    KeyValue = compilationDebugIsEnabled,
                    KeyDefaultValue = false
                },
                new WebConfigSettingsKey
                {
                    KeyName = "Trace",
                    KeyDisplayName = Metadata.Terms.WebConfig.TraceKeyDisplayName,
                    KeyValue = traceIsEnabled,
                    KeyDefaultValue = false
                }
            };

            results.Data.Add(webConfigSettingsKeys.AsResult().WithLabel(Metadata.Terms.TableNames.WebConfig));

            return results;
        }
    }
}