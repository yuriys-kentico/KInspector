using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Models;
using KenticoInspector.Reports.ContentTreeConsistencyAnalysis;
using KenticoInspector.Reports.ContentTreeConsistencyAnalysis.Models;
using KenticoInspector.Reports.ContentTreeConsistencyAnalysis.Models.Data;
using KenticoInspector.Reports.ContentTreeConsistencyAnalysis.Models.Results;
using KenticoInspector.Reports.Tests.Helpers;

using NUnit.Framework;

namespace KenticoInspector.Reports.Tests
{
    [TestFixture(10)]
    [TestFixture(11)]
    [TestFixture(12)]
    public class ContentTreeConsistencyAnalysisTests : AbstractReportTest<Report, Terms>
    {
        private Report _mockReport;

        private List<CmsDocument> CmsDocumentNodesWithIssues => new List<CmsDocument>
        {
                new CmsDocument {
                    DocumentID = 100,
                    DocumentName = "Bad 100",
                    DocumentNamePath = "/bad-100",
                    DocumentNodeID = 100
                },
                new CmsDocument {
                    DocumentID = 150,
                    DocumentName = "Bad 150",
                    DocumentNamePath = "/bad-150",
                    DocumentNodeID = 150
                }
        };

        private List<CmsTreeNode> CmsTreeNodesWithissues => new List<CmsTreeNode>
        {
            new CmsTreeNode {
                ClassDisplayName = "Bad Class",
                ClassName = "BadClass",
                NodeAliasPath = "/bad-1",
                NodeClassID = 1234,
                NodeID = 101,
                NodeLevel = 1,
                NodeName = "bad-1",
                NodeParentID = 0,
                NodeSiteID = 1
            }
        };

        public ContentTreeConsistencyAnalysisTests(int majorVersion) : base(majorVersion)
        {
            _mockReport = new Report(_mockDatabaseService.Object, _mockReportMetadataService.Object);
        }

        [Test]
        public void Should_ReturnErrorResult_When_ThereAreDocumentsWithMissingTreeNode()
        {
            // Arrange
            SetupAllDatabaseQueries(documentsWithMissingTreeNode: CmsDocumentNodesWithIssues);

            // Act
            var results = _mockReport.GetResults();

            // Assert
            Assert.That(results.Status == ReportResultsStatus.Error);
        }

        [Test]
        public void Should_ReturnErrorResult_When_ThereAreTreeNodesWithBadParentNode()
        {
            // Arrange
            SetupAllDatabaseQueries(treeNodesWithBadParentNodeId: CmsTreeNodesWithissues);

            // Act
            var results = _mockReport.GetResults();

            // Assert
            Assert.That(results.Status == ReportResultsStatus.Error);
        }

        [Test]
        public void Should_ReturnErrorResult_When_ThereAreTreeNodesWithBadParentSite()
        {
            // Arrange
            SetupAllDatabaseQueries(treeNodesWithBadParentSiteId: CmsTreeNodesWithissues);

            // Act
            var results = _mockReport.GetResults();

            // Assert
            Assert.That(results.Status == ReportResultsStatus.Error);
        }

        [Test]
        public void Should_ReturnErrorResult_When_ThereAreTreeNodesWithDuplicatedAliasPath()
        {
            // Arrange
            SetupAllDatabaseQueries(treeNodesWithDuplicatedAliasPath: CmsTreeNodesWithissues);

            // Act
            var results = _mockReport.GetResults();

            // Assert
            Assert.That(results.Status == ReportResultsStatus.Error);
        }

        [Test]
        public void Should_ReturnErrorResult_When_ThereAreTreeNodesWithLevelMismatchByAliasPathTest()
        {
            // Arrange
            SetupAllDatabaseQueries(treeNodesWithLevelMismatchByAliasPathTest: CmsTreeNodesWithissues);

            // Act
            var results = _mockReport.GetResults();

            // Assert
            Assert.That(results.Status == ReportResultsStatus.Error);
        }

        [Test]
        public void Should_ReturnErrorResult_When_ThereAreTreeNodesWithLevelMismatchByNodeLevelTest()
        {
            // Arrange
            SetupAllDatabaseQueries(treeNodesWithLevelMismatchByNodeLevelTest: CmsTreeNodesWithissues);

            // Act
            var results = _mockReport.GetResults();

            // Assert
            Assert.That(results.Status == ReportResultsStatus.Error);
        }

        [Test]
        public void Should_ReturnErrorResult_When_ThereAreTreeNodesWithMissingDocument()
        {
            // Arrange
            SetupAllDatabaseQueries(treeNodesWithMissingDocument: CmsTreeNodesWithissues);

            // Act
            var results = _mockReport.GetResults();

            // Assert
            Assert.That(results.Status == ReportResultsStatus.Error);
        }

        [Test]
        public void Should_ReturnErrorResult_When_ThereAreTreeNodesWithPageTypeNotAssignedToSite()
        {
            // Arrange
            SetupAllDatabaseQueries(treeNodesWithPageTypeNotAssignedToSite: CmsTreeNodesWithissues);

            // Act
            var results = _mockReport.GetResults();

            // Assert
            Assert.That(results.Status == ReportResultsStatus.Error);
        }

        [Test]
        public void Should_ReturnErrorResult_When_ThereAreVersionHistoryIssues()
        {
            // Arrange
            SetupAllDatabaseQueries(isVersionHistoryDataSetClean: false);

            // Act
            var results = _mockReport.GetResults();

            // Assert
            Assert.That(results.Status == ReportResultsStatus.Error, $"Status was '{results.Status}' instead of 'Error'");

            var rowCount = results.GetAnonymousTableResult<TableResult<VersionHistoryMismatchResult>>(_mockReport.Metadata.Terms.TableNames.WorkflowInconsistencies).Rows.Count();

            Assert.That(rowCount == 4, $"There were {rowCount} rows instead 4 as expected");
        }

        [Test]
        public void Should_ReturnGoodResult_When_DatabaseHasNoIssues()
        {
            // Arrange
            SetupAllDatabaseQueries();

            // Act
            var results = _mockReport.GetResults();

            // Assert
            Assert.That(results.Status == ReportResultsStatus.Good);
        }

        private void SetupAllDatabaseQueries(
            List<CmsDocument> documentsWithMissingTreeNode = null,
            List<CmsTreeNode> treeNodesWithBadParentNodeId = null,
            List<CmsTreeNode> treeNodesWithBadParentSiteId = null,
            List<CmsTreeNode> treeNodesWithDuplicatedAliasPath = null,
            List<CmsTreeNode> treeNodesWithLevelMismatchByAliasPathTest = null,
            List<CmsTreeNode> treeNodesWithLevelMismatchByNodeLevelTest = null,
            List<CmsTreeNode> treeNodesWithMissingDocument = null,
            List<CmsTreeNode> treeNodesWithPageTypeNotAssignedToSite = null,
            bool isVersionHistoryDataSetClean = true
            )
        {
            documentsWithMissingTreeNode = documentsWithMissingTreeNode ?? new List<CmsDocument>();
            SetupCmsDocumentNodeQueries(documentsWithMissingTreeNode, Scripts.GetDocumentIdsWithMissingTreeNode);

            treeNodesWithBadParentNodeId = treeNodesWithBadParentNodeId ?? new List<CmsTreeNode>();
            SetupCmsTreeNodeQueries(treeNodesWithBadParentNodeId, Scripts.GetTreeNodeIdsWithBadParentNodeId);

            treeNodesWithBadParentSiteId = treeNodesWithBadParentSiteId ?? new List<CmsTreeNode>();
            SetupCmsTreeNodeQueries(treeNodesWithBadParentSiteId, Scripts.GetTreeNodeIdsWithBadParentSiteId);

            treeNodesWithDuplicatedAliasPath = treeNodesWithDuplicatedAliasPath ?? new List<CmsTreeNode>();
            SetupCmsTreeNodeQueries(treeNodesWithDuplicatedAliasPath, Scripts.GetTreeNodeIdsWithDuplicatedAliasPath);

            treeNodesWithLevelMismatchByAliasPathTest = treeNodesWithLevelMismatchByAliasPathTest ?? new List<CmsTreeNode>();
            SetupCmsTreeNodeQueries(treeNodesWithLevelMismatchByAliasPathTest, Scripts.GetTreeNodeIdsWithLevelMismatchByAliasPathTest);

            treeNodesWithLevelMismatchByNodeLevelTest = treeNodesWithLevelMismatchByNodeLevelTest ?? new List<CmsTreeNode>();
            SetupCmsTreeNodeQueries(treeNodesWithLevelMismatchByNodeLevelTest, Scripts.GetTreeNodeIdsWithLevelMismatchByNodeLevelTest);

            treeNodesWithMissingDocument = treeNodesWithMissingDocument ?? new List<CmsTreeNode>();
            SetupCmsTreeNodeQueries(treeNodesWithMissingDocument, Scripts.GetTreeNodeIdsWithMissingDocument);

            treeNodesWithPageTypeNotAssignedToSite = treeNodesWithPageTypeNotAssignedToSite ?? new List<CmsTreeNode>();
            SetupCmsTreeNodeQueries(treeNodesWithPageTypeNotAssignedToSite, Scripts.GetTreeNodeIdsWithPageTypeNotAssignedToSite);

            var versionHistoryDataSet = new VersionHistoryDataSet(isVersionHistoryDataSetClean);

            SetupCmsVersionHistoryQueries(versionHistoryDataSet.CmsVersionHistoryItems, Scripts.GetLatestVersionHistoryIdForAllDocuments);

            SetupCmsClassItemsQueries(versionHistoryDataSet.CmsClassItems);

            SetupVersionHistoryCoupledDataQueries(versionHistoryDataSet.CmsVersionHistoryItems, versionHistoryDataSet.CmsClassItems, versionHistoryDataSet.VersionHistoryCoupledData);
        }

        private void SetupVersionHistoryCoupledDataQueries(List<CmsVersionHistoryItem> versionHistoryItems, List<CmsClass> versionHistoryCmsClassItems, List<IDictionary<string, object>> versionHistoryCoupledData)
        {
            foreach (var cmsClassItem in versionHistoryCmsClassItems)
            {
                var coupledDataIds = versionHistoryItems
                    .Where(versionHistoryItem => versionHistoryItem.VersionClassID == cmsClassItem.ClassID)
                    .Select(versionHistoryItem => versionHistoryItem.CoupledDataID);

                var returnedItems = versionHistoryCoupledData
                    .Where(versionHistoryCoupledDataItem => coupledDataIds
                        .Contains((int)versionHistoryCoupledDataItem[cmsClassItem.ClassIDColumn])
                    );

                var replacements = new Dictionary<string, string>
                {
                    { "TableName", cmsClassItem.ClassTableName },
                    { "IdColumnName", cmsClassItem.ClassIDColumn }
                };

                _mockDatabaseService.SetupExecuteSqlFromFileGenericWithListParameter(
                    Scripts.GetCmsDocumentCoupledDataItems,
                    replacements,
                    "coupledDataIds",
                    coupledDataIds,
                    returnedItems
                );
            }
        }

        private void SetupCmsClassItemsQueries(IEnumerable<CmsClass> returnedItems, string idScript = null)
        {
            var idValues = returnedItems.Select(x => x.ClassID);
            SetupDetailsAndIdQueries(idValues, returnedItems, idScript, Scripts.GetCmsClass, "cmsClassIds");
        }

        private void SetupCmsDocumentNodeQueries(IEnumerable<CmsDocument> returnedItems, string idScript = null)
        {
            var idValues = returnedItems.Select(x => x.DocumentID);
            SetupDetailsAndIdQueries(idValues, returnedItems, idScript, Scripts.GetDocumentNodeDetails);
        }

        private void SetupCmsTreeNodeQueries(IEnumerable<CmsTreeNode> returnedItems, string idScript = null)
        {
            var idValues = returnedItems.Select(x => x.NodeID);
            SetupDetailsAndIdQueries(idValues, returnedItems, idScript, Scripts.GetTreeNodeDetails);
        }

        private void SetupCmsVersionHistoryQueries(IEnumerable<CmsVersionHistoryItem> returnedItems, string idScript = null)
        {
            var idValues = returnedItems.Select(x => x.VersionHistoryID);
            SetupDetailsAndIdQueries(idValues, returnedItems, idScript, Scripts.GetVersionHistoryDetails, "latestVersionHistoryIds");
        }

        private void SetupDetailsAndIdQueries<T>(IEnumerable<int> idValues, IEnumerable<T> returnedItems, string idScript, string detailsScript, string parameterName = "nodeIds")
        {
            if (idValues == null)
            {
                throw new ArgumentNullException("idValues");
            }

            if (!string.IsNullOrWhiteSpace(idScript))
            {
                _mockDatabaseService.SetupExecuteSqlFromFile(idScript, idValues);
            }

            if (!string.IsNullOrWhiteSpace(detailsScript) && returnedItems != null)
            {
                _mockDatabaseService.SetupExecuteSqlFromFileWithListParameter(detailsScript, parameterName, idValues, returnedItems);
            }
        }

        private class VersionHistoryDataSet
        {
            public List<CmsVersionHistoryItem> CmsVersionHistoryItems { get; set; }

            public List<CmsClass> CmsClassItems { get; set; }

            public List<IDictionary<string, object>> VersionHistoryCoupledData { get; set; }

            public VersionHistoryDataSet(bool clean = true)
            {
                var classFormDefinitionXml5512 = new XmlDocument();
                classFormDefinitionXml5512.Load("TestData/classFormDefinitionXml_Clean_5512.xml");

                CmsClassItems = new List<CmsClass>
                {
                    new CmsClass
                    {
                        ClassDisplayName = "",
                        ClassFormDefinitionXml = new XmlDocument(),
                        ClassID = 5512,
                        ClassName = "KIN.VersioningDataTest",
                        ClassTableName = "KIN_VersioningDataTest"
                    }
                };

                CmsClassItems[0].ClassFormDefinitionXml = classFormDefinitionXml5512;

                CmsVersionHistoryItems = new List<CmsVersionHistoryItem>();
                VersionHistoryCoupledData = new List<IDictionary<string, object>>();

                if (clean)
                {
                    var versionHistoryXml = new XmlDocument();
                    versionHistoryXml.Load("TestData/VersionHistoryItem_Clean_518.xml");

                    CmsVersionHistoryItems.Add(new CmsVersionHistoryItem
                    {
                        DocumentID = 518,
                        NodeXml = versionHistoryXml,
                        VersionClassID = 5512,
                        VersionHistoryID = 17,
                        WasPublishedFrom = DateTime.Parse("2019-06-06 11:58:49.2430968")
                    });

                    var coupledData = new Dictionary<string, object>
                    {
                        { "VersioningDataTestID", 5 },
                        { "BoolNoDefault", false },
                        { "BoolDefaultTrue", true },
                        { "BoolDefaultFalse", false },
                        { "DateTimeNoDefault", null },
                        { "DateTimeHardDefault", DateTime.Parse("2019-06-06 11:31:17.0000000") },
                        { "DateTimeMacroDefault", DateTime.Parse("2019-06-06 11:58:33.0000000") },
                        { "TextNoDefault", null },
                        { "TextHardDefault", "This is the default" },
                        { "DecimalNoDefault", null },
                        { "DecimalHardDefault", 1.7500m }
                    };

                    VersionHistoryCoupledData.Add(coupledData);
                }
                else
                {
                    var versionHistoryXml = new XmlDocument();
                    versionHistoryXml.Load("TestData/VersionHistoryItem_Corrupt_519.xml");

                    CmsVersionHistoryItems.Add(new CmsVersionHistoryItem
                    {
                        DocumentID = 519,
                        NodeXml = versionHistoryXml,
                        VersionClassID = 5512,
                        VersionHistoryID = 18,
                        WasPublishedFrom = DateTime.Parse("2019-06-14 10:46:18.4493088")
                    });

                    var coupledData = new Dictionary<string, object>
                    {
                        { "VersioningDataTestID", 6 },
                        { "BoolNoDefault", true },
                        { "BoolDefaultTrue", false },
                        { "BoolDefaultFalse", false },
                        { "DateTimeNoDefault", DateTime.Parse("2019-06-14 10:54:35.0000000") },
                        { "DateTimeHardDefault", DateTime.Parse("2019-06-14 10:45:36.0000000") },
                        { "DateTimeMacroDefault", DateTime.Parse("2019-06-14 10:45:37.0000000") },
                        { "TextNoDefault", "Text 1 (corrupted)" },
                        { "TextHardDefault", "Text 2" },
                        { "DecimalNoDefault", 1.0150m },
                        { "DecimalHardDefault", 1.0200m }
                    };

                    VersionHistoryCoupledData.Add(coupledData);
                }
            }
        }
    }
}