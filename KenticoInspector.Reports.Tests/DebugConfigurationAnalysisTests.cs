using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using KenticoInspector.Core.Constants;
using KenticoInspector.Reports.DebugConfigurationAnalysis;
using KenticoInspector.Reports.DebugConfigurationAnalysis.Models;
using KenticoInspector.Reports.DebugConfigurationAnalysis.Models.Data;

using NUnit.Framework;

namespace KenticoInspector.Reports.Tests
{
    [TestFixture(10)]
    [TestFixture(11)]
    [TestFixture(12)]
    public class DebugConfigurationAnalysisTests : AbstractReportTest<Report, Terms>
    {
        private readonly Report _mockReport;

        private static readonly string webConfigXml = @"<configuration><system.web><compilation debug=""false"" /></system.web></configuration>";
        private static readonly string webConfigXmlWithCompilationDebug = @"<configuration><system.web><compilation debug=""true"" /></system.web></configuration>";
        private static readonly string webConfigXmlWithCompilationDebugAndTrace = @"<configuration><system.web><compilation debug=""falue"" /><trace enabled=""true"" /></system.web></configuration>";

        public DebugConfigurationAnalysisTests(int majorVersion) : base(majorVersion)
        {
            _mockReport = new Report(_mockDatabaseService.Object, _mockInstanceService.Object, _mockCmsFileService.Object, _mockReportMetadataService.Object);
        }

        [Test]
        public void Should_ReturnErrorStatus_When_DebugEnabledInWebConfig()
        {
            // Arrange
            ArrangeServices(customWebconfigXml: webConfigXmlWithCompilationDebug);

            // Act
            var results = _mockReport.GetResults();

            // Assert
            Assert.That(results.Status == ReportResultsStatus.Error, "When debug is enabled in the web.config, the report status should be 'error'");
        }

        [Test]
        public void Should_ReturnErrorStatus_When_TraceEnabledInWebConfig()
        {
            // Arrange
            ArrangeServices(customWebconfigXml: webConfigXmlWithCompilationDebugAndTrace);

            // Act
            var results = _mockReport.GetResults();

            // Assert
            Assert.That(results.Status == ReportResultsStatus.Error, "When trace is enabled in the web.config, the report status should be 'error'");
        }

        [Test]
        public void Should_ReturnInformationStatus_When_ResultsHaveNoIssues()
        {
            // Arrange
            ArrangeServices();

            // Act
            var results = _mockReport.GetResults();

            // Assert
            Assert.That(results.Status == ReportResultsStatus.Good, "When the results are clean, the report status should be 'good'");
        }

        [Test]
        public void Should_ReturnWarningStatus_When_AnyDatabaseSettingIsTrueAndNotTheDefaultValue()
        {
            // Arrange
            var settingsKey = new CmsSettingsKey("CMSDebugEverything", "Enable all debugs", true, false);
            ArrangeServices(customDatabaseSettingsValues: new[] { settingsKey });

            // Act
            var results = _mockReport.GetResults();

            // Assert
            Assert.That(results.Status == ReportResultsStatus.Warning, "When any database setting is set to true and that isn't the default value, the report status should be 'warning'");
        }

        private void ArrangeServices(IEnumerable<CmsSettingsKey> customDatabaseSettingsValues = null, string customWebconfigXml = null)
        {
            ArrangeResourceStringsMethods();
            ArrangeWebConfigMethods(customWebconfigXml);
            ArrangeDatabaseSettingsMethods(customDatabaseSettingsValues);
        }

        private void ArrangeResourceStringsMethods()
        {
            _mockCmsFileService
                .Setup(p => p.GetResourceStringsFromResx(_mockInstance.Path, DefaultKenticoPaths.PrimaryResxFile))
                .Returns(new Dictionary<string, string>());
        }

        private void ArrangeWebConfigMethods(string customWebconfigXml)
        {
            var webconfigXml = !string.IsNullOrWhiteSpace(customWebconfigXml) ? customWebconfigXml : webConfigXml;

            var webConfig = XDocument.Parse(webconfigXml);

            _mockCmsFileService
                .Setup(p => p.GetXDocument(_mockInstance.Path, DefaultKenticoPaths.WebConfigFile))
                .Returns(webConfig);
        }

        private void ArrangeDatabaseSettingsMethods(IEnumerable<CmsSettingsKey> customDatabaseSettingsValues)
        {
            var databaseSettingsKeyValuesResults = GetDatabaseSettingsKeyValuesResults(customDatabaseSettingsValues);

            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<CmsSettingsKey>(Scripts.GetCMSSettingsKeysForDebug))
                .Returns(databaseSettingsKeyValuesResults);
        }

        private List<CmsSettingsKey> GetDatabaseSettingsKeyValuesResults(IEnumerable<CmsSettingsKey> customSettingsKeyValues = null)
        {
            var results = new List<CmsSettingsKey>();

            if (customSettingsKeyValues != null)
            {
                results.AddRange(customSettingsKeyValues);
            }

            AddDefaultDatabaseSettingsKeyValues(results);

            return results;
        }

        private void AddDefaultDatabaseSettingsKeyValues(List<CmsSettingsKey> results)
        {
            var defaultDatabaseSettingsKeyValues = new List<CmsSettingsKey>
            {
                new CmsSettingsKey("CMSDebugAnalytics", "Enable web analytics debug", false, false),
                new CmsSettingsKey("CMSDebugCache", "Enable cache access debug", false, false),
                new CmsSettingsKey("CMSDebugEverything", "Enable all debugs", false, false),
                new CmsSettingsKey("CMSDebugEverythingEverywhere", "Debug everything everywhere", false, false),
                new CmsSettingsKey("CMSDebugFiles", "Enable IO operation debug", false, false),
                new CmsSettingsKey("CMSDebugHandlers", "Enable handlers debug", false, false),
                new CmsSettingsKey("CMSDebugImportExport", "Debug Import/Export", false, false),
                new CmsSettingsKey("CMSDebugMacros", "Enable macro debug", false, false),
                new CmsSettingsKey("CMSDebugOutput", "Enable output debug", false, false),
                new CmsSettingsKey("CMSDebugRequests", "Enable request debug", false, false),
                new CmsSettingsKey("CMSDebugResources", "Debug resources", false, false),
                new CmsSettingsKey("CMSDebugScheduler", "Debug scheduler", true, true),
                new CmsSettingsKey("CMSDebugSecurity", "Enable security debug", false, false),
                new CmsSettingsKey("CMSDebugSQLConnections", "Debug SQL connections", false, false),
                new CmsSettingsKey("CMSDebugSQLQueries", "Enable SQL query debug", false, false),
                new CmsSettingsKey("CMSDebugViewState", "Enable ViewState debug", false, false),
                new CmsSettingsKey("CMSDebugWebFarm", "Enable web farm debug", false, false),
                new CmsSettingsKey("CMSDisableDebug", "Disable debugging", false, false)
            };

            foreach (var settingsKey in defaultDatabaseSettingsKeyValues)
            {
                var keyNameMatchCount = results
                    .Count(x => x.KeyName == settingsKey.KeyName);

                if (keyNameMatchCount == 0)
                {
                    results.Add(settingsKey);
                }
            }
        }
    }
}