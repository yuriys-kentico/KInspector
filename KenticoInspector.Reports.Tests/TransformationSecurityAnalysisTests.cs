using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

using KenticoInspector.Core.Tests.Mocks;
using KenticoInspector.Core.Modules.Models.Results;
using KenticoInspector.Core.Modules.Models.Results.Data;
using KenticoInspector.Reports.Tests.AbstractClasses;
using KenticoInspector.Reports.TransformationSecurityAnalysis;
using KenticoInspector.Reports.TransformationSecurityAnalysis.Models;
using KenticoInspector.Reports.TransformationSecurityAnalysis.Models.Data;
using KenticoInspector.Reports.TransformationSecurityAnalysis.Models.Results;

using NUnit.Framework;

namespace KenticoInspector.Reports.Tests
{
    [TestFixture(10)]
    [TestFixture(11)]
    [TestFixture(12)]
    public class TransformationSecurityAnalysisTests : AbstractReportTests<Report, Terms>
    {
        private readonly Report mockReport;

        private IEnumerable<CmsTreeNode> TreeNodesWithoutIssues => new List<CmsTreeNode>
        {
            new CmsTreeNode
            {
                DocumentName = "Page Using ASCX Page Template",
                DocumentCulture = "en-US",
                NodeAliasPath = "/path/to/page-using-template",
                DocumentPageTemplateID = 1
            },
            new CmsTreeNode
            {
                DocumentName = "Another Page Using ASCX Page Template",
                DocumentCulture = "en-US",
                NodeAliasPath = "/path/to/another/page-using-template",
                DocumentPageTemplateID = 1
            },
            new CmsTreeNode
            {
                DocumentName = "Page Using Text Page Template",
                DocumentCulture = "es-ES",
                NodeAliasPath = "/path/to/page-using-text-template",
                DocumentPageTemplateID = 2
            }
        };

        private IEnumerable<CmsPageTemplate> PageTemplateWithoutIssues => new List<CmsPageTemplate>
        {
            new CmsPageTemplate
            {
                PageTemplateID = 1,
                PageTemplateWebParts =
                    ParseXDocumentFromFile(@"TestData\CMS_PageTemplate\PageTemplateWebParts\CleanAscx.xml"),
                PageTemplateCodeName = "PageTemplateASCX"
            },
            new CmsPageTemplate
            {
                PageTemplateID = 2,
                PageTemplateWebParts =
                    ParseXDocumentFromFile(@"TestData\CMS_PageTemplate\PageTemplateWebParts\CleanText.xml"),
                PageTemplateCodeName = "PageTemplateText"
            }
        };

        private IEnumerable<CmsTransformation> TransformationWithoutIssues => new List<CmsTransformation>
        {
            new CmsTransformation
            {
                TransformationName = "ASCXTransformation",
                TransformationCode = FromFile(@"TestData\CMS_Transformation\TransformationCode\CleanASCX.txt"),
                ClassName = "PageType1",
                TransformationType = "ASCX"
            },
            new CmsTransformation
            {
                TransformationName = "JQueryTransformation",
                TransformationCode = FromFile(@"TestData\CMS_Transformation\TransformationCode\CleanText.txt"),
                ClassName = "PageType1",
                TransformationType = "JQuery"
            },
            new CmsTransformation
            {
                TransformationName = "TextTransformation",
                TransformationCode = FromFile(@"TestData\CMS_Transformation\TransformationCode\CleanText.txt"),
                ClassName = "PageType2",
                TransformationType = "Text"
            }
        };

        public TransformationSecurityAnalysisTests(int majorVersion) : base(majorVersion)
        {
            mockReport = ArrangeProperties(
                new Report(
                    mockDatabaseService.Object,
                    mockInstanceService.Object
                    )
                );
        }

        private static XDocument ParseXDocumentFromFile(string path) => XDocument.Parse(FromFile(path));

        private static string FromFile(string path) => File.ReadAllText(path);

        public void TestSingleIssue(
            string transformationCodeFilePath,
            Func<TransformationResult, dynamic, bool> transformationResultEvaluator
            )
        {
            var transformationWithIssue = new List<CmsTransformation>
            {
                new CmsTransformation
                {
                    TransformationName = "ASCXTransformation",
                    TransformationCode = FromFile(transformationCodeFilePath),
                    ClassName = "PageType1",
                    TransformationType = "ASCX"
                }
            };

            // Arrange
            ArrangeDatabaseService(transformationWithIssue);

            // Act
            var results = mockReport.GetResults();

            // Assert
            Assert.That(
                results.Status,
                Is.EqualTo(ResultsStatus.Warning)
                );

            Assert.That(
                results.Data.First<TableResult<IssueTypeResult>>()
                    .Rows.Count(),
                Is.EqualTo(1)
                );

            Assert.That(
                results.Data.First<TableResult<TransformationResult>>()
                    .Rows.Count(),
                Is.EqualTo(1)
                );

            Assert.That(
                results.Data.First<TableResult<TransformationResult>>()
                    .Rows,
                Has.One.Matches<TransformationResult>(
                    row => transformationResultEvaluator(
                        row,
                        row as dynamic
                        )
                    )
                );

            Assert.That(
                results.Data.First<TableResult<TransformationUsageResult>>()
                    .Rows.Count(),
                Is.EqualTo(1)
                );

            Assert.That(
                results.Data.First<TableResult<TemplateUsageResult>>()
                    .Rows.Count(),
                Is.EqualTo(2)
                );
        }

        private void ArrangeDatabaseService(IEnumerable<CmsTransformation> transformation)
        {
            mockDatabaseService.SetupExecuteSqlFromFile(
                Scripts.GetTransformations,
                transformation
                );

            mockDatabaseService.SetupExecuteSqlFromFile(
                Scripts.GetTreeNodes,
                TreeNodesWithoutIssues
                );

            mockDatabaseService.SetupExecuteSqlFromFile(
                Scripts.GetPageTemplates,
                "DocumentPageTemplateIDs",
                TreeNodesWithoutIssues.Select(treeNode => treeNode.DocumentPageTemplateID),
                PageTemplateWithoutIssues
                );
        }

        [Test]
        public void Should_ReturnGoodStatusAndGoodSummary_When_TransformationsWithoutIssues()
        {
            // Arrange
            ArrangeDatabaseService(TransformationWithoutIssues);

            // Act
            var results = mockReport.GetResults();

            // Assert
            Assert.That(
                results.Status,
                Is.EqualTo(ResultsStatus.Good)
                );

            Assert.That(
                results.Summary,
                Is.EqualTo(mockReport.Metadata.Terms.GoodSummary.ToString())
                );
        }

        [Test]
        public void Should_ReturnWarningResult_When_TransformationsWithSingleXssQueryHelperIssue()
        {
            TestSingleIssue(
                @"TestData\CMS_Transformation\TransformationCode\WithXssQueryHelperIssueASCX.txt",
                (
                    r,
                    d
                    ) => d.XssQueryHelper != string.Empty && r.TransformationUses == 2
                );
        }
    }
}