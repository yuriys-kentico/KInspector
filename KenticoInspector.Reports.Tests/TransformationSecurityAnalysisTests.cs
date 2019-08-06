using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Models;
using KenticoInspector.Reports.Tests.Helpers;
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
    public class TransformationSecurityAnalysisTests : AbstractReportTest<Report, Terms>
    {
        private readonly Report mockReport;

        private IEnumerable<CmsTreeNode> TreeNodesWithoutIssues => new List<CmsTreeNode>
        {
            new CmsTreeNode()
            {
                DocumentName = "Page Using ASCX Page Template",
                DocumentCulture = "en-US",
                NodeAliasPath = "/path/to/page-using-template",
                DocumentPageTemplateID = 1
            },
            new CmsTreeNode()
            {
                DocumentName = "Another Page Using ASCX Page Template",
                DocumentCulture = "en-US",
                NodeAliasPath = "/path/to/another/page-using-template",
                DocumentPageTemplateID = 1
            },
            new CmsTreeNode()
            {
                DocumentName = "Page Using Text Page Template",
                DocumentCulture = "es-ES",
                NodeAliasPath = "/path/to/page-using-text-template",
                DocumentPageTemplateID = 2
            }
        };

        private IEnumerable<CmsPageTemplate> CmsPageTemplateWithoutIssues => new List<CmsPageTemplate>
        {
            new CmsPageTemplate()
            {
                PageTemplateID = 1,
                PageTemplateWebParts = ParseXDocumentFromFile(@"TestData\CMS_PageTemplate\PageTemplateWebParts\CleanAscx.xml"),
                PageTemplateCodeName = "PageTemplateASCX"
            },
            new CmsPageTemplate()
            {
                PageTemplateID = 2,
                PageTemplateWebParts = ParseXDocumentFromFile(@"TestData\CMS_PageTemplate\PageTemplateWebParts\CleanText.xml"),
                PageTemplateCodeName = "PageTemplateText"
            }
        };

        private IEnumerable<CmsTransformation> CmsTransformationWithoutIssues => new List<CmsTransformation>
        {
            new CmsTransformation()
            {
                TransformationName = "ASCXTransformation",
                TransformationCode = FromFile(@"TestData\CMS_Transformation\TransformationCode\CleanASCX.txt"),
                ClassName = "PageType1",
                TransformationType = TransformationType.ASCX
            },
            new CmsTransformation()
            {
                TransformationName = "JQueryTransformation",
                TransformationCode = FromFile(@"TestData\CMS_Transformation\TransformationCode\CleanText.txt"),
                ClassName = "PageType1",
                TransformationType = TransformationType.JQuery
            },
            new CmsTransformation()
            {
                TransformationName = "TextTransformation",
                TransformationCode = FromFile(@"TestData\CMS_Transformation\TransformationCode\CleanText.txt"),
                ClassName = "PageType2",
                TransformationType = TransformationType.Text
            }
        };

        public TransformationSecurityAnalysisTests(int majorVersion) : base(majorVersion)
        {
            mockReport = new Report(_mockDatabaseService.Object, _mockReportMetadataService.Object, _mockInstanceService.Object);
        }

        private static XDocument ParseXDocumentFromFile(string path)
        {
            return XDocument.Parse(FromFile(path));
        }

        private static string FromFile(string path)
        {
            return File.ReadAllText(path);
        }

        [Test]
        public void Should_ReturnGoodStatusAndGoodSummary_When_TransformationsHaveNoIssues()
        {
            // Arrange
            ArrangeDatabaseService(CmsTransformationWithoutIssues);

            // Act
            var results = mockReport.GetResults();

            // Assert
            Assert.That(results.Status, Is.EqualTo(ReportResultsStatus.Good));

            Assert.That(results.Summary, Is.EqualTo(mockReport.Metadata.Terms.GoodSummary.ToString()));
        }

        [Test]
        public void Should_ReturnWarningStatus_When_TransformationsHaveSingleXssQueryHelperIssue() => TestSingleIssue(@"TestData\CMS_Transformation\TransformationCode\WithXssQueryHelperIssueASCX.txt", (r, d) => d.XssQueryHelper != string.Empty && r.TransformationUses == 2);

        public void TestSingleIssue(string transformationCodeFilePath, Func<TransformationResult, dynamic, bool> transformationResultEvaluator)
        {
            var cmsTransformationWithIssue = new List<CmsTransformation>
            {
                    new CmsTransformation()
                {
                    TransformationName = "ASCXTransformation",
                    TransformationCode = FromFile(transformationCodeFilePath),
                    ClassName = "PageType1",
                    TransformationType = TransformationType.ASCX
                }
            };

            // Arrange
            ArrangeDatabaseService(cmsTransformationWithIssue);

            // Act
            var results = mockReport.GetResults();

            // Assert
            Assert.That(results.Status, Is.EqualTo(ReportResultsStatus.Warning));

            Assert.That(results.GetAnonymousTableResult<TableResult<IssueTypeResult>>("issueTypesResult").Rows.Count(), Is.EqualTo(1));
            Assert.That(results.GetAnonymousTableResult<TableResult<TransformationResult>>("transformationsResult").Rows.Count(), Is.EqualTo(1));
            Assert.That(results.GetAnonymousTableResult<TableResult<TransformationResult>>("transformationsResult").Rows, Has.One.Matches<TransformationResult>(row => transformationResultEvaluator(row, row as dynamic)));

            Assert.That(results.GetAnonymousTableResult<TableResult<TransformationUsageResult>>("transformationUsageResult").Rows.Count(), Is.EqualTo(1));

            Assert.That(results.GetAnonymousTableResult<TableResult<TemplateUsageResult>>("templateUsageResult").Rows.Count(), Is.EqualTo(2));
        }

        private void ArrangeDatabaseService(IEnumerable<CmsTransformation> cmsTransformation)
        {
            _mockDatabaseService.SetupExecuteSqlFromFile(Scripts.GetTransformations, cmsTransformation);
            _mockDatabaseService.SetupExecuteSqlFromFile(Scripts.GetTreeNodes, TreeNodesWithoutIssues);
            _mockDatabaseService.SetupExecuteSqlFromFileWithListParameter(Scripts.GetPageTemplates, "DocumentPageTemplateIDs", TreeNodesWithoutIssues.Select(treeNode => treeNode.DocumentPageTemplateID), CmsPageTemplateWithoutIssues);
        }
    }
}