using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using KenticoInspector.Core.Instances;
using KenticoInspector.Core.Instances.Models;
using KenticoInspector.Core.Instances.Services;
using KenticoInspector.Core.Modules;
using KenticoInspector.Core.Modules.Models.Results;
using KenticoInspector.Core.Modules.Models.Results.Data;
using KenticoInspector.Modules;
using KenticoInspector.Reports.SecuritySettingsAnalysis.Analyzers;
using KenticoInspector.Reports.SecuritySettingsAnalysis.Models;
using KenticoInspector.Reports.SecuritySettingsAnalysis.Models.Data;
using KenticoInspector.Reports.SecuritySettingsAnalysis.Models.Results;

using static KenticoInspector.Core.Modules.Models.Tags;

namespace KenticoInspector.Reports.SecuritySettingsAnalysis
{
    public class Report : AbstractReport<Terms>
    {
        private readonly IDatabaseService databaseService;
        private readonly IInstanceService instanceService;
        private readonly ICmsFileService cmsFileService;

        public Report(
            IDatabaseService databaseService,
            IInstanceService instanceService,
            ICmsFileService cmsFileService
            )
        {
            this.databaseService = databaseService;
            this.instanceService = instanceService;
            this.cmsFileService = cmsFileService;
        }

        [Tags(Security)]
        [SupportsVersions("10 - 12.0")]
        public override ReportResults GetResults()
        {
            var cmsSettingsKeysNames = new SettingsKeyAnalyzers(null!)
                .Analyzers
                .Select(analyzer => analyzer.Parameters[0].Name);

            var cmsSettingsKeys = databaseService.ExecuteSqlFromFile<CmsSettingsKey>(
                Scripts.GetSecurityCmsSettings,
                new { cmsSettingsKeysNames }
                );

            var cmsSettingsKeyResults = GetCmsSettingsKeyResults(cmsSettingsKeys);

            var cmsSettingsCategoryIdsOnPaths = cmsSettingsKeyResults
                .SelectMany(cmsSettingsKeyResult => cmsSettingsKeyResult.GetCategoryIdsOnPath())
                .Distinct();

            var cmsSettingsCategories = databaseService.ExecuteSqlFromFile<CmsSettingsCategory>(
                Scripts.GetCmsSettingsCategories,
                new { cmsSettingsCategoryIdsOnPaths }
                );

            var sites = instanceService
                .GetInstanceDetails()
                .Sites
                .Append(new CmsSite
                {
                    SiteId = 0,
                    SiteName = Metadata.Terms.GlobalSiteName
                });

            var instancePath = instanceService.CurrentInstance.Path;

            var resxValues = cmsFileService.GetResourceStringsFromResx(instancePath, DefaultKenticoPaths.PrimaryResxFile);

            var localizedCmsSettingsKeyResults = cmsSettingsKeyResults
                .Select(cmsSettingsKeyResult => new CmsSettingsKeyResult(
                    cmsSettingsKeyResult,
                    cmsSettingsCategories,
                    sites,
                    resxValues
                    ));

            var webConfigXml = cmsFileService.GetXDocument(instancePath, DefaultKenticoPaths.WebConfigFile);

            var webConfigSettingsResults = GetWebConfigSettingsResults(webConfigXml);

            return CompileResults(localizedCmsSettingsKeyResults, webConfigSettingsResults);
        }

        private IEnumerable<CmsSettingsKeyResult> GetCmsSettingsKeyResults(IEnumerable<CmsSettingsKey> cmsSettingsKeys)
        {
            var analyzersObject = new SettingsKeyAnalyzers(Metadata.Terms);

            foreach (var analyzer in analyzersObject.Analyzers)
            {
                var analysisResult = analyzersObject.GetAnalysis(analyzer, cmsSettingsKeys, key => key.KeyName);

                if (analysisResult is CmsSettingsKeyResult cmsSettingsKeyResult)
                {
                    yield return cmsSettingsKeyResult;
                }
            }
        }

        private IEnumerable<WebConfigSettingResult> GetWebConfigSettingsResults(XDocument webConfigXml)
        {
            var appSettings = webConfigXml
                .Descendants("appSettings")
                .Elements();

            var appSettingsResults = GetAppSettingsResults(appSettings);

            var systemWebElements = webConfigXml
                .Descendants("system.web")
                .Elements();

            var systemWebSettingsResults = GetSystemWebSettingsResults(systemWebElements);

            var connectionStrings = webConfigXml
                .Descendants("connectionStrings")
                .Elements();

            var connectionStringElementsResults = GetConnectionStringsResults(connectionStrings);

            var webConfigSettingsResults = appSettingsResults
                .Concat(systemWebSettingsResults)
                .Concat(connectionStringElementsResults);

            return webConfigSettingsResults;
        }

        private IEnumerable<WebConfigSettingResult> GetAppSettingsResults(IEnumerable<XElement> appSettings)
        {
            var analyzersObject = new AppSettingAnalyzers(Metadata.Terms);

            foreach (var analyzer in analyzersObject.Analyzers)
            {
                var analysisResult = analyzersObject.GetAnalysis(analyzer, appSettings, key => key.Attribute("key")?.Value);

                if (analysisResult is WebConfigSettingResult appSettingResult)
                {
                    yield return appSettingResult;
                }
            }
        }

        private IEnumerable<WebConfigSettingResult> GetSystemWebSettingsResults(IEnumerable<XElement> systemWebElements)
        {
            var analyzersObject = new SystemWebSettingAnalyzers(Metadata.Terms);

            foreach (var analyzer in analyzersObject.Analyzers)
            {
                var analysisResult = analyzersObject.GetAnalysis(analyzer, systemWebElements, key => key.Name.LocalName);

                if (analysisResult is WebConfigSettingResult systemWebElementsResult)
                {
                    yield return systemWebElementsResult;
                }
            }
        }

        private IEnumerable<WebConfigSettingResult> GetConnectionStringsResults(IEnumerable<XElement> connectionStringElements)
        {
            var analyzersObject = new ConnectionStringAnalyzers(Metadata.Terms);

            foreach (var analyzer in analyzersObject.Analyzers)
            {
                var analysisResult = analyzersObject.GetAnalysis(
                    analyzer,
                    connectionStringElements,
                    key => key.Attribute("name")?.Value
                    );

                if (analysisResult is WebConfigSettingResult connectionStringResult)
                {
                    yield return connectionStringResult;
                }
            }
        }

        private ReportResults CompileResults(
            IEnumerable<CmsSettingsKeyResult> cmsSettingsKeyResults,
            IEnumerable<WebConfigSettingResult> webConfigSettingsResults
            )
        {
            if (!cmsSettingsKeyResults.Any() && !webConfigSettingsResults.Any())
            {
                return new ReportResults(ResultsStatus.Good)
                {
                    Summary = Metadata.Terms.Summaries.Good
                };
            }

            var cmsSettingsKeyResultsCount = cmsSettingsKeyResults.Count();
            var webConfigSettingsResultsCount = webConfigSettingsResults.Count();

            return new ReportResults(ResultsStatus.Warning)
            {
                Summary = Metadata.Terms.Summaries.Warning.With(new
                {
                    cmsSettingsKeyResultsCount,
                    webConfigSettingsResultsCount
                }),
                Data =
                {
                    cmsSettingsKeyResults.AsResult().WithLabel(Metadata.Terms.TableTitles.AdminSecuritySettings),
                    webConfigSettingsResults.AsResult().WithLabel(Metadata.Terms.TableTitles.WebConfigSecuritySettings)
                }
            };
        }
    }
}