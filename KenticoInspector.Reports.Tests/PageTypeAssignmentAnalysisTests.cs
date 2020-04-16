using System.Collections.Generic;
using System.Linq;

using KenticoInspector.Core.Modules.Models.Results;
using KenticoInspector.Core.Modules.Models.Results.Data;
using KenticoInspector.Reports.PageTypeAssignmentAnalysis;
using KenticoInspector.Reports.PageTypeAssignmentAnalysis.Models;
using KenticoInspector.Reports.PageTypeAssignmentAnalysis.Models.Data;
using KenticoInspector.Reports.Tests.AbstractClasses;

using NUnit.Framework;

namespace KenticoInspector.Reports.Tests
{
    [TestFixture(10)]
    [TestFixture(11)]
    [TestFixture(12)]
    public class PageTypeAssignmentAnalysisTests : AbstractReportTests<Report, Terms>
    {
        private readonly Report mockReport;

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
            }
        };

        private IEnumerable<CmsPageType> PageTypesWithoutIssues => new List<CmsPageType>();

        public PageTypeAssignmentAnalysisTests(int majorVersion) : base(majorVersion)
        {
            mockReport = ArrangeProperties(new Report(mockDatabaseService.Object));
        }

        private void ArrangeDatabaseService(IEnumerable<CmsPageType> unassignedPageTypes)
        {
            mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<CmsPageType>(Scripts.GetPageTypesNotAssignedToSite))
                .Returns(unassignedPageTypes);
        }

        [Test]
        public void Should_ReturnEmptyListOfIdenticalLayouts_When_NoneFound()
        {
            // Arrange
            ArrangeDatabaseService(PageTypesWithoutIssues);

            // Act
            var results = mockReport.GetResults();

            // Assert
            Assert.That(
                results.Status,
                Is.EqualTo(ResultsStatus.Good)
                );
        }

        [Test]
        public void Should_ReturnWarningResult_When_PageTypesWithIssues()
        {
            // Arrange
            ArrangeDatabaseService(PageTypesWithIssues);

            // Act
            var results = mockReport.GetResults();

            // Assert
            Assert.That(
                results.Data.First<TableResult<CmsPageType>>()
                    .Rows.Count(),
                Is.GreaterThan(0)
                );

            Assert.That(
                results.Status,
                Is.EqualTo(ResultsStatus.Warning)
                );
        }
    }
}