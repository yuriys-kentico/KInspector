using KenticoInspector.Core.Constants;
using KenticoInspector.Reports.PageTypeAssignmentAnalysis;
using KenticoInspector.Reports.PageTypeAssignmentAnalysis.Models;
using KenticoInspector.Reports.PageTypeAssignmentAnalysis.Models.Data;
using NUnit.Framework;
using System.Collections.Generic;

namespace KenticoInspector.Reports.Tests
{
    [TestFixture(10)]
    [TestFixture(11)]
    [TestFixture(12)]
    public class PageTypeAssignmentAnalysisTests : AbstractReportTest<Report, Terms>
    {
        private readonly Report _mockReport;

        private IEnumerable<CmsPageType> PageTypesWithIssues => new List<CmsPageType>
        {
            new CmsPageType
            {
                ClassName = "DancingGoatMvc.Article",
                ClassDisplayName = "Article (MVC)",
                NodeSiteID = 1,
                NodeClassID = 5494
            },
            new CmsPageType
            {
                ClassName = "DancingGoatMvc.Brewer",
                ClassDisplayName = "Brewer (MVC)",
                NodeSiteID = 1,
                NodeClassID = 5477
            },
            new CmsPageType
            {
                ClassName = "CMS.News",
                ClassDisplayName = "News",
                NodeSiteID = 2,
                NodeClassID = 5502
            },
            new CmsPageType
            {
                ClassName = "CMS.Office",
                ClassDisplayName = "Office",
                NodeSiteID = 2,
                NodeClassID = 5514
            },
            new CmsPageType
            {
                ClassName = "globaltypes.customtype",
                ClassDisplayName = "Custom Type",
                NodeSiteID = 2,
                NodeClassID = 5497
            },
        };

        private IEnumerable<CmsPageType> PageTypesWithoutIssues => new List<CmsPageType>
        {
        };

        public PageTypeAssignmentAnalysisTests(int majorVersion) : base(majorVersion)
        {
            _mockReport = new Report(_mockDatabaseService.Object, _mockReportMetadataService.Object);
        }

        [Test]
        public void Should_ReturnWarningResult_When_PageTypesWithIssues()
        {
            // Arrange
            ArrangeDatabaseService(PageTypesWithIssues);

            // Act
            var results = _mockReport.GetResults();

            // Assert
            Assert.That(results.Data.Rows.Count, Is.GreaterThan(0));
            Assert.That(results.Status, Is.EqualTo(ReportResultsStatus.Warning));
        }

        [Test]
        public void Should_ReturnEmptyListOfIdenticalLayouts_When_NoneFound()
        {
            // Arrange
            ArrangeDatabaseService(PageTypesWithoutIssues);

            // Act
            var results = _mockReport.GetResults();

            // Assert
            Assert.That(results.Status, Is.EqualTo(ReportResultsStatus.Good));
        }

        private void ArrangeDatabaseService(IEnumerable<CmsPageType> unassignedPageTypes)
        {
            _mockDatabaseService
               .Setup(p => p.ExecuteSqlFromFile<CmsPageType>(Scripts.GetPageTypesNotAssignedToSite))
               .Returns(unassignedPageTypes);
        }
    }
}