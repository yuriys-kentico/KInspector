﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using KenticoInspector.Core.Tests.Mocks;
using KenticoInspector.Core.Modules.Models.Results;
using KenticoInspector.Core.Modules.Models.Results.Data;
using KenticoInspector.Reports.ContentTreeConsistencyAnalysis;
using KenticoInspector.Reports.ContentTreeConsistencyAnalysis.Models;
using KenticoInspector.Reports.ContentTreeConsistencyAnalysis.Models.Data;
using KenticoInspector.Reports.ContentTreeConsistencyAnalysis.Models.Results;
using KenticoInspector.Reports.Tests.AbstractClasses;

using NUnit.Framework;

namespace KenticoInspector.Reports.Tests
{
    [TestFixture(10)]
    [TestFixture(11)]
    [TestFixture(12)]
    public class ContentTreeConsistencyAnalysisTests : AbstractReportTests<Report, Terms>
    {
        private readonly Report mockReport;

        private IEnumerable<CmsDocument> DocumentNodesWithIssues => new List<CmsDocument>
        {
            new CmsDocument
            {
                DocumentID = 100,
                DocumentName = "Bad 100",
                DocumentNamePath = "/bad-100",
                DocumentNodeID = 100
            },
            new CmsDocument
            {
                DocumentID = 150,
                DocumentName = "Bad 150",
                DocumentNamePath = "/bad-150",
                DocumentNodeID = 150
            }
        };

        private IEnumerable<CmsTreeNode> TreeNodesWithissues => new List<CmsTreeNode>
        {
            new CmsTreeNode
            {
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
            mockReport = ArrangeProperties(new Report(mockDatabaseService.Object));
        }

        private void SetupAllDatabaseQueries(
            IEnumerable<CmsDocument>? documentsWithMissingTreeNode = null,
            IEnumerable<CmsTreeNode>? treeNodesWithBadParentNodeId = null,
            IEnumerable<CmsTreeNode>? treeNodesWithBadParentSiteId = null,
            IEnumerable<CmsTreeNode>? treeNodesWithDuplicatedAliasPath = null,
            IEnumerable<CmsTreeNode>? treeNodesWithLevelMismatchByAliasPathTest = null,
            IEnumerable<CmsTreeNode>? treeNodesWithLevelMismatchByNodeLevelTest = null,
            IEnumerable<CmsTreeNode>? treeNodesWithMissingDocument = null,
            IEnumerable<CmsTreeNode>? treeNodesWithPageTypeNotAssignedToSite = null,
            bool isVersionHistoryDataSetClean = true
            )
        {
            documentsWithMissingTreeNode ??= new List<CmsDocument>();

            SetupCmsDocumentNodeQueries(
                documentsWithMissingTreeNode,
                Scripts.GetDocumentIdsWithMissingTreeNode
                );

            treeNodesWithBadParentNodeId ??= new List<CmsTreeNode>();

            SetupCmsTreeNodeQueries(
                treeNodesWithBadParentNodeId,
                Scripts.GetTreeNodeIdsWithBadParentNodeId
                );

            treeNodesWithBadParentSiteId ??= new List<CmsTreeNode>();

            SetupCmsTreeNodeQueries(
                treeNodesWithBadParentSiteId,
                Scripts.GetTreeNodeIdsWithBadParentSiteId
                );

            treeNodesWithDuplicatedAliasPath ??= new List<CmsTreeNode>();

            SetupCmsTreeNodeQueries(
                treeNodesWithDuplicatedAliasPath,
                Scripts.GetTreeNodeIdsWithDuplicatedAliasPath
                );

            treeNodesWithLevelMismatchByAliasPathTest ??= new List<CmsTreeNode>();

            SetupCmsTreeNodeQueries(
                treeNodesWithLevelMismatchByAliasPathTest,
                Scripts.GetTreeNodeIdsWithLevelMismatchByAliasPath
                );

            treeNodesWithLevelMismatchByNodeLevelTest ??= new List<CmsTreeNode>();

            SetupCmsTreeNodeQueries(
                treeNodesWithLevelMismatchByNodeLevelTest,
                Scripts.GetTreeNodeIdsWithLevelMismatchByNodeLevel
                );

            treeNodesWithMissingDocument ??= new List<CmsTreeNode>();

            SetupCmsTreeNodeQueries(
                treeNodesWithMissingDocument,
                Scripts.GetTreeNodeIdsWithMissingDocument
                );

            treeNodesWithPageTypeNotAssignedToSite ??= new List<CmsTreeNode>();

            SetupCmsTreeNodeQueries(
                treeNodesWithPageTypeNotAssignedToSite,
                Scripts.GetTreeNodeIdsWithPageTypeNotAssignedToSite
                );

            var versionHistoryDataSet = new VersionHistoryDataSet(isVersionHistoryDataSetClean);

            SetupCmsVersionHistoryQueries(
                versionHistoryDataSet.CmsVersionHistoryItems,
                Scripts.GetLatestVersionHistoryIdForAllDocuments
                );

            SetupCmsClassItemsQueries(versionHistoryDataSet.CmsClassItems);

            SetupVersionHistoryCoupledDataQueries(
                versionHistoryDataSet.CmsVersionHistoryItems,
                versionHistoryDataSet.CmsClassItems,
                versionHistoryDataSet.VersionHistoryCoupledData
                );
        }

        private void SetupVersionHistoryCoupledDataQueries(
            List<CmsVersionHistoryItem> versionHistoryItems,
            List<CmsClass> versionHistoryClasses,
            List<IDictionary<string, object?>> versionHistoryCoupledData
            )
        {
            foreach (var cmsClass in versionHistoryClasses)
                if (cmsClass.ClassIDColumn != null && cmsClass.ClassTableName != null)
                {
                    var coupledDataIds = versionHistoryItems
                        .Where(versionHistoryItem => versionHistoryItem.VersionClassID == cmsClass.ClassID)
                        .Select(versionHistoryItem => versionHistoryItem.CoupledDataID);

                    var returnedItems = versionHistoryCoupledData
                        .Where(
                            versionHistoryCoupledDataItem =>
                            {
                                var value = versionHistoryCoupledDataItem[cmsClass.ClassIDColumn];

                                return value != null && coupledDataIds.Contains((int)value);
                            }
                            );

                    var replacements = new Dictionary<string, string>
                    {
                        {"TableName", cmsClass.ClassTableName},
                        {"IdColumnName", cmsClass.ClassIDColumn}
                    };

                    mockDatabaseService.SetupExecuteSqlFromFile(
                        Scripts.GetCmsDocumentCoupledDataItems,
                        replacements,
                        "coupledDataIds",
                        coupledDataIds,
                        returnedItems
                        );
                }
        }

        private void SetupCmsClassItemsQueries(
            IEnumerable<CmsClass> returnedItems,
            string? idScript = null
            )
        {
            var idValues = returnedItems.Select(x => x.ClassID);

            SetupDetailsAndIdQueries(
                idValues,
                returnedItems,
                idScript,
                Scripts.GetCmsClass,
                "idsBatch"
                );
        }

        private void SetupCmsDocumentNodeQueries(
            IEnumerable<CmsDocument> returnedItems,
            string? idScript = null
            )
        {
            var idValues = returnedItems.Select(x => x.DocumentID);

            SetupDetailsAndIdQueries(
                idValues,
                returnedItems,
                idScript,
                Scripts.GetDocumentNodeDetails,
                "nodeIds"
                );
        }

        private void SetupCmsTreeNodeQueries(
            IEnumerable<CmsTreeNode> returnedItems,
            string? idScript = null
            )
        {
            var idValues = returnedItems.Select(x => x.NodeID);

            SetupDetailsAndIdQueries(
                idValues,
                returnedItems,
                idScript,
                Scripts.GetTreeNodeDetails,
                "nodeIds"
                );
        }

        private void SetupCmsVersionHistoryQueries(
            IEnumerable<CmsVersionHistoryItem> returnedItems,
            string? idScript = null
            )
        {
            var idValues = returnedItems.Select(x => x.VersionHistoryID);

            SetupDetailsAndIdQueries(
                idValues,
                returnedItems,
                idScript,
                Scripts.GetVersionHistoryDetails,
                "idsBatch"
                );
        }

        private void SetupDetailsAndIdQueries<T>(
            IEnumerable<int> idValues,
            IEnumerable<T> returnedItems,
            string? idScript,
            string detailsScript,
            string parameterName
            )
        {
            if (idValues == null) throw new ArgumentNullException(nameof(idValues));

            if (!string.IsNullOrWhiteSpace(idScript))
                mockDatabaseService.SetupExecuteSqlFromFile(
                    idScript,
                    idValues
                    );

            if (!string.IsNullOrWhiteSpace(detailsScript) && returnedItems != null)
                mockDatabaseService.SetupExecuteSqlFromFile(
                    detailsScript,
                    parameterName,
                    idValues,
                    returnedItems
                    );
        }

        private class VersionHistoryDataSet
        {
            public List<CmsVersionHistoryItem> CmsVersionHistoryItems { get; }

            public List<CmsClass> CmsClassItems { get; }

            public List<IDictionary<string, object?>> VersionHistoryCoupledData { get; }

            public VersionHistoryDataSet(bool clean = true)
            {
                var classFormDefinitionXml5512 = XDocument.Load("TestData/classFormDefinitionXml_Clean_5512.xml");

                CmsClassItems = new List<CmsClass>
                {
                    new CmsClass
                    {
                        ClassDisplayName = "",
                        ClassFormDefinitionXml = classFormDefinitionXml5512,
                        ClassID = 5512,
                        ClassName = "KIN.VersioningDataTest",
                        ClassTableName = "KIN_VersioningDataTest"
                    }
                };

                CmsVersionHistoryItems = new List<CmsVersionHistoryItem>();
                VersionHistoryCoupledData = new List<IDictionary<string, object?>>();

                if (clean)
                {
                    var versionHistoryXml = XDocument.Load("TestData/VersionHistoryItem_Clean_518.xml");

                    CmsVersionHistoryItems.Add(
                        new CmsVersionHistoryItem
                        {
                            DocumentID = 518,
                            NodeXml = versionHistoryXml,
                            VersionClassID = 5512,
                            VersionHistoryID = 17,
                            WasPublishedFrom = DateTime.Parse("2019-06-06 11:58:49.2430968")
                        }
                        );

                    var coupledData = new Dictionary<string, object?>
                    {
                        {"VersioningDataTestID", 5},
                        {"BoolNoDefault", false},
                        {"BoolDefaultTrue", true},
                        {"BoolDefaultFalse", false},
                        {"DateTimeNoDefault", null},
                        {"DateTimeHardDefault", DateTime.Parse("2019-06-06 11:31:17.0000000")},
                        {"DateTimeMacroDefault", DateTime.Parse("2019-06-06 11:58:33.0000000")},
                        {"TextNoDefault", null},
                        {"TextHardDefault", "This is the default"},
                        {"DecimalNoDefault", null},
                        {"DecimalHardDefault", 1.7500m}
                    };

                    VersionHistoryCoupledData.Add(coupledData);
                }
                else
                {
                    var versionHistoryXml = XDocument.Load("TestData/VersionHistoryItem_Corrupt_519.xml");

                    CmsVersionHistoryItems.Add(
                        new CmsVersionHistoryItem
                        {
                            DocumentID = 519,
                            NodeXml = versionHistoryXml,
                            VersionClassID = 5512,
                            VersionHistoryID = 18,
                            WasPublishedFrom = DateTime.Parse("2019-06-14 10:46:18.4493088")
                        }
                        );

                    var coupledData = new Dictionary<string, object?>
                    {
                        {"VersioningDataTestID", 6},
                        {"BoolNoDefault", true},
                        {"BoolDefaultTrue", false},
                        {"BoolDefaultFalse", false},
                        {"DateTimeNoDefault", DateTime.Parse("2019-06-14 10:54:35.0000000")},
                        {"DateTimeHardDefault", DateTime.Parse("2019-06-14 10:45:36.0000000")},
                        {"DateTimeMacroDefault", DateTime.Parse("2019-06-14 10:45:37.0000000")},
                        {"TextNoDefault", "Text 1 (corrupted)"},
                        {"TextHardDefault", "Text 2"},
                        {"DecimalNoDefault", 1.0150m},
                        {"DecimalHardDefault", 1.0200m}
                    };

                    VersionHistoryCoupledData.Add(coupledData);
                }
            }
        }

        [Test]
        public void Should_ReturnErrorResult_When_DocumentsWithMissingTreeNode()
        {
            // Arrange
            SetupAllDatabaseQueries(DocumentNodesWithIssues);

            // Act
            var results = mockReport.GetResults();

            // Assert
            Assert.That(
                results.Status,
                Is.EqualTo(ResultsStatus.Error)
                );
        }

        [Test]
        public void Should_ReturnErrorResult_When_TreeNodesWithBadParentNode()
        {
            // Arrange
            SetupAllDatabaseQueries(treeNodesWithBadParentNodeId: TreeNodesWithissues);

            // Act
            var results = mockReport.GetResults();

            // Assert
            Assert.That(
                results.Status,
                Is.EqualTo(ResultsStatus.Error)
                );
        }

        [Test]
        public void Should_ReturnErrorResult_When_TreeNodesWithBadParentSite()
        {
            // Arrange
            SetupAllDatabaseQueries(treeNodesWithBadParentSiteId: TreeNodesWithissues);

            // Act
            var results = mockReport.GetResults();

            // Assert
            Assert.That(
                results.Status,
                Is.EqualTo(ResultsStatus.Error)
                );
        }

        [Test]
        public void Should_ReturnErrorResult_When_TreeNodesWithDuplicatedAliasPath()
        {
            // Arrange
            SetupAllDatabaseQueries(treeNodesWithDuplicatedAliasPath: TreeNodesWithissues);

            // Act
            var results = mockReport.GetResults();

            // Assert
            Assert.That(
                results.Status,
                Is.EqualTo(ResultsStatus.Error)
                );
        }

        [Test]
        public void Should_ReturnErrorResult_When_TreeNodesWithLevelMismatchByAliasPathTest()
        {
            // Arrange
            SetupAllDatabaseQueries(treeNodesWithLevelMismatchByAliasPathTest: TreeNodesWithissues);

            // Act
            var results = mockReport.GetResults();

            // Assert
            Assert.That(
                results.Status,
                Is.EqualTo(ResultsStatus.Error)
                );
        }

        [Test]
        public void Should_ReturnErrorResult_When_TreeNodesWithLevelMismatchByNodeLevelTest()
        {
            // Arrange
            SetupAllDatabaseQueries(treeNodesWithLevelMismatchByNodeLevelTest: TreeNodesWithissues);

            // Act
            var results = mockReport.GetResults();

            // Assert
            Assert.That(
                results.Status,
                Is.EqualTo(ResultsStatus.Error)
                );
        }

        [Test]
        public void Should_ReturnErrorResult_When_TreeNodesWithMissingDocument()
        {
            // Arrange
            SetupAllDatabaseQueries(treeNodesWithMissingDocument: TreeNodesWithissues);

            // Act
            var results = mockReport.GetResults();

            // Assert
            Assert.That(
                results.Status,
                Is.EqualTo(ResultsStatus.Error)
                );
        }

        [Test]
        public void Should_ReturnErrorResult_When_TreeNodesWithPageTypeNotAssignedToSite()
        {
            // Arrange
            SetupAllDatabaseQueries(treeNodesWithPageTypeNotAssignedToSite: TreeNodesWithissues);

            // Act
            var results = mockReport.GetResults();

            // Assert
            Assert.That(
                results.Status,
                Is.EqualTo(ResultsStatus.Error)
                );
        }

        [Test]
        public void Should_ReturnErrorResult_When_VersionHistoryWithIssues()
        {
            // Arrange
            SetupAllDatabaseQueries(isVersionHistoryDataSetClean: false);

            // Act
            var results = mockReport.GetResults();

            // Assert
            Assert.That(
                results.Status,
                Is.EqualTo(ResultsStatus.Error)
                );

            Assert.That(
                results.Data.First<TableResult<VersionHistoryMismatchResult>>()
                    .Rows.Count(),
                Is.EqualTo(4)
                );
        }

        [Test]
        public void Should_ReturnGoodResult_When_DatabaseWithoutIssues()
        {
            // Arrange
            SetupAllDatabaseQueries();

            // Act
            var results = mockReport.GetResults();

            // Assert
            Assert.That(
                results.Status,
                Is.EqualTo(ResultsStatus.Good)
                );
        }
    }
}