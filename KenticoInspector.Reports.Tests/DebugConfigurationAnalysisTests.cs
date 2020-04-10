using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using KenticoInspector.Core.Instances;
using KenticoInspector.Core.Modules.Models.Results;
using KenticoInspector.Reports.DebugConfigurationAnalysis;
using KenticoInspector.Reports.DebugConfigurationAnalysis.Models;
using KenticoInspector.Reports.DebugConfigurationAnalysis.Models.Data;

using NUnit.Framework;

namespace KenticoInspector.Reports.Tests
{
    [TestFixture(10)]
    [TestFixture(11)]
    [TestFixture(12)]
    public class DebugConfigurationAnalysisTests : AbstractReportTests<Report, Terms>
    {
        private readonly Report mockReport;

        private const string webConfigXml = @"<configuration><system.web><compilation debug=""false"" /></system.web></configuration>";
        private const string webConfigXmlWithCompilationDebug = @"<configuration><system.web><compilation debug=""true"" /></system.web></configuration>";
        private const string webConfigXmlWithCompilationDebugAndTrace = @"<configuration><system.web><compilation debug=""falue"" /><trace enabled=""true"" /></system.web></configuration>";

        public DebugConfigurationAnalysisTests(int majorVersion) : base(majorVersion)
        {
            mockReport = ArrangeProperties(new Report(mockDatabaseService.Object, mockInstanceService.Object, mockCmsFileService.Object));
        }

        [Test]
        public void Should_ReturnInformationResult_When_DebugConfigurationWithoutIssues()
        {
            // Arrange
            ArrangeServices();

            // Act
            var results = mockReport.GetResults();

            // Assert
            Assert.That(results.Status, Is.EqualTo(ResultsStatus.Good));
        }

        [Test]
        public void Should_ReturnErrorResult_When_DebugEnabledInWebConfig()
        {
            // Arrange
            ArrangeServices(customWebconfigXml: webConfigXmlWithCompilationDebug);

            // Act
            var results = mockReport.GetResults();

            // Assert
            Assert.That(results.Status, Is.EqualTo(ResultsStatus.Error));
        }

        [Test]
        public void Should_ReturnErrorResult_When_TraceEnabledInWebConfig()
        {
            // Arrange
            ArrangeServices(customWebconfigXml: webConfigXmlWithCompilationDebugAndTrace);

            // Act
            var results = mockReport.GetResults();

            // Assert
            Assert.That(results.Status, Is.EqualTo(ResultsStatus.Error));
        }

        [Test]
        public void Should_ReturnWarningResult_When_AnyDebugConfigurationIsTrueAndNotTheDefaultValue()
        {
            // Arrange
            var settingsKey = new CmsSettingsKey
            {
                KeyName = "CMSDebugEverything",
                KeyDisplayName = "Enable all debugs",
                KeyValue = true,
                KeyDefaultValue = false
            };

            ArrangeServices(customDatabaseSettingsValues: new[] { settingsKey });

            // Act
            var results = mockReport.GetResults();

            // Assert
            Assert.That(results.Status, Is.EqualTo(ResultsStatus.Warning));
        }

        private void ArrangeServices(IEnumerable<CmsSettingsKey>? customDatabaseSettingsValues = null, string? customWebconfigXml = null)
        {
            ArrangeResourceStringsMethods();
            ArrangeWebConfigMethods(customWebconfigXml);
            ArrangeDatabaseSettingsMethods(customDatabaseSettingsValues);
        }

        private void ArrangeResourceStringsMethods()
        {
            mockCmsFileService
                .Setup(p => p.GetResourceStringsFromResx(mockInstance.Path, DefaultKenticoPaths.PrimaryResxFile))
                .Returns(new Dictionary<string, string>());
        }

        private void ArrangeWebConfigMethods(string? customWebconfigXml)
        {
            var webconfigXml = !string.IsNullOrWhiteSpace(customWebconfigXml) ? customWebconfigXml : webConfigXml;

            var webConfig = XDocument.Parse(webconfigXml);

            mockCmsFileService
                .Setup(p => p.GetXDocument(mockInstance.Path, DefaultKenticoPaths.WebConfigFile))
                .Returns(webConfig);
        }

        private void ArrangeDatabaseSettingsMethods(IEnumerable<CmsSettingsKey>? customDatabaseSettingsValues)
        {
            var databaseSettingsKeyValuesResults = GetDatabaseSettingsKeyValuesResults(customDatabaseSettingsValues);

            mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<CmsSettingsKey>(Scripts.GetCMSSettingsKeysForDebug))
                .Returns(databaseSettingsKeyValuesResults);
        }

        private List<CmsSettingsKey> GetDatabaseSettingsKeyValuesResults(IEnumerable<CmsSettingsKey>? customSettingsKeyValues = null)
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
                new CmsSettingsKey
                {
                    KeyName = "CMSDebugAnalytics",
                    KeyDisplayName = "Enable web analytics debug",
                    KeyValue = false,
                    KeyDefaultValue = false
                },
                new CmsSettingsKey
                {
                    KeyName = "CMSDebugCache",
                    KeyDisplayName = "Enable cache access debug",
                    KeyValue = false,
                    KeyDefaultValue = false
                },
                new CmsSettingsKey
                {
                    KeyName = "CMSDebugEverything",
                    KeyDisplayName = "Enable all debugs",
                    KeyValue = false,
                    KeyDefaultValue = false
                },
                new CmsSettingsKey
                {
                    KeyName = "CMSDebugEverythingEverywhere",
                    KeyDisplayName = "Debug everything everywhere",
                    KeyValue = false,
                    KeyDefaultValue = false
                },
                new CmsSettingsKey
                {
                    KeyName = "CMSDebugFiles",
                    KeyDisplayName = "Enable IO operation debug",
                    KeyValue = false,
                    KeyDefaultValue = false
                },
                new CmsSettingsKey
                {
                    KeyName = "CMSDebugHandlers",
                    KeyDisplayName = "Enable handlers debug",
                    KeyValue = false,
                    KeyDefaultValue = false
                },
                new CmsSettingsKey
                {
                    KeyName = "CMSDebugImportExport",
                    KeyDisplayName = "Debug Import/Export",
                    KeyValue = false,
                    KeyDefaultValue = false
                },
                new CmsSettingsKey
                {
                    KeyName = "CMSDebugMacros",
                    KeyDisplayName = "Enable macro debug",
                    KeyValue = false,
                    KeyDefaultValue = false
                },
                new CmsSettingsKey
                {
                    KeyName = "CMSDebugOutput",
                    KeyDisplayName = "Enable output debug",
                    KeyValue = false,
                    KeyDefaultValue = false
                },
                new CmsSettingsKey
                {
                    KeyName = "CMSDebugRequests",
                    KeyDisplayName = "Enable request debug",
                    KeyValue = false,
                    KeyDefaultValue = false
                },
                new CmsSettingsKey
                {
                    KeyName = "CMSDebugResources",
                    KeyDisplayName = "Debug resources",
                    KeyValue = false,
                    KeyDefaultValue = false
                },
                new CmsSettingsKey
                {
                    KeyName = "CMSDebugScheduler",
                    KeyDisplayName = "Debug scheduler",
                    KeyValue = true,
                    KeyDefaultValue = true
                },
                new CmsSettingsKey
                {
                    KeyName = "CMSDebugSecurity",
                    KeyDisplayName = "Enable security debug",
                    KeyValue = false,
                    KeyDefaultValue = false
                },
                new CmsSettingsKey
                {
                    KeyName = "CMSDebugSQLConnections",
                    KeyDisplayName = "Debug SQL connections",
                    KeyValue = false,
                    KeyDefaultValue = false
                },
                new CmsSettingsKey
                {
                    KeyName = "CMSDebugSQLQueries",
                    KeyDisplayName = "Enable SQL query debug",
                    KeyValue = false,
                    KeyDefaultValue = false
                },
                new CmsSettingsKey
                {
                    KeyName = "CMSDebugViewState",
                    KeyDisplayName = "Enable ViewState debug",
                    KeyValue = false,
                    KeyDefaultValue = false
                },
                new CmsSettingsKey
                {
                    KeyName = "CMSDebugWebFarm",
                    KeyDisplayName = "Enable web farm debug",
                    KeyValue = false,
                    KeyDefaultValue = false
                },
                new CmsSettingsKey
                {
                    KeyName = "CMSDisableDebug",
                    KeyDisplayName = "Disable debugging",
                    KeyValue = false,
                    KeyDefaultValue = false
                }
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