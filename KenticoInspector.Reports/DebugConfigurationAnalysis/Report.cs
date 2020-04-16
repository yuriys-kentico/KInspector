using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using KenticoInspector.Core.Instances;
using KenticoInspector.Core.Instances.Services;
using KenticoInspector.Core.Modules;
using KenticoInspector.Core.Modules.Models.Results;
using KenticoInspector.Core.Modules.Models.Results.Data;
using KenticoInspector.Modules;
using KenticoInspector.Reports.DebugConfigurationAnalysis.Models;
using KenticoInspector.Reports.DebugConfigurationAnalysis.Models.Data;

using static KenticoInspector.Core.Modules.Models.Tags;

namespace KenticoInspector.Reports.DebugConfigurationAnalysis
{
    public class Report : AbstractReport<Terms>
    {
        private readonly ICmsFileService _cmsFileService;
        private readonly IDatabaseService _databaseService;
        private readonly IInstanceService _instanceService;

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

        [Tags(Health)]
        [SupportsVersions("10 - 12.0")]
        public override ReportResults GetResults()
        {
            var instance = _instanceService.CurrentInstance;

            var databaseSettingsValues =
                _databaseService.ExecuteSqlFromFile<CmsSettingsKey>(Scripts.GetCMSSettingsKeysForDebug);

            var resxValues =
                _cmsFileService.GetResourceStringsFromResx(
                    instance.Path,
                    DefaultKenticoPaths.PrimaryResxFile
                    );

            ResolveSettingsDisplayNames(
                databaseSettingsValues,
                resxValues
                );

            var webConfig = _cmsFileService.GetXDocument(
                instance.Path,
                DefaultKenticoPaths.WebConfigFile
                );

            var compilationDebugIsEnabled =
                GetBooleanValueofSectionAttribute(
                    webConfig,
                    "system.web",
                    "compilation",
                    "debug"
                    );

            var traceIsEnabled = GetBooleanValueofSectionAttribute(
                webConfig,
                "system.web",
                "trace",
                "enabled"
                );

            return CompileResults(
                databaseSettingsValues,
                compilationDebugIsEnabled,
                traceIsEnabled
                );
        }

        private static void ResolveSettingsDisplayNames(
            IEnumerable<CmsSettingsKey> databaseSettingsValues,
            IDictionary<string, string> resxValues
            )
        {
            foreach (var databaseSettingsValue in databaseSettingsValues)
            {
                var key = databaseSettingsValue.KeyDisplayName
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
                    key,
                    out var value
                    )) databaseSettingsValue.KeyDisplayName = value;
            }
        }

        private static bool GetBooleanValueofSectionAttribute(
            XDocument webConfig,
            string subSection,
            string element,
            string attributeName
            )
        {
            var valueRaw = webConfig
                .Descendants(subSection)
                .FirstOrDefault()
                .Element(element)
                ?
                .Attribute(attributeName)
                ?
                .Value;

            bool.TryParse(
                valueRaw,
                out var value
                );

            return value;
        }

        private ReportResults CompileResults(
            IEnumerable<CmsSettingsKey> databaseSettingsKeys,
            bool compilationDebugIsEnabled,
            bool traceIsEnabled
            )
        {
            var explicitlyEnabledSettings = databaseSettingsKeys
                .Where(
                    databaseSettingsKey =>
                        databaseSettingsKey.KeyValue && databaseSettingsKey.KeyDefaultValue == false
                    );

            var compilationDebugAndTraceAreEnabled = compilationDebugIsEnabled || traceIsEnabled;

            if (!explicitlyEnabledSettings.Any() && !compilationDebugAndTraceAreEnabled)
                return new ReportResults(ResultsStatus.Good)
                {
                    Summary = Metadata.Terms.GoodSummary
                };

            var results = new ReportResults(ResultsStatus.Warning);

            if (explicitlyEnabledSettings.Any())
            {
                var explicitlyEnabledSettingsCount = explicitlyEnabledSettings.Count();

                results.Summary += Metadata.Terms.WarningSummary.With(
                    new
                    {
                        explicitlyEnabledSettingsCount
                    }
                    );

                results.Data.Add(
                    explicitlyEnabledSettings.AsResult()
                        .WithLabel(Metadata.Terms.TableNames.ExplicitlyEnabledSettings)
                    );
            }

            results.Data.Add(
                databaseSettingsKeys.AsResult()
                    .WithLabel(Metadata.Terms.TableNames.Overview)
                );

            if (compilationDebugAndTraceAreEnabled)
            {
                results.Status = ResultsStatus.Error;

                results.Summary += Metadata.Terms.ErrorSummary.With(
                    new
                    {
                        compilationDebugIsEnabled,
                        traceIsEnabled
                    }
                    );
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

            results.Data.Add(
                webConfigSettingsKeys.AsResult()
                    .WithLabel(Metadata.Terms.TableNames.WebConfig)
                );

            return results;
        }
    }
}