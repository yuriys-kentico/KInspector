using System.Collections.Generic;

using KenticoInspector.Core.Constants;
using KenticoInspector.Reports.TemplateLayoutAnalysis;
using KenticoInspector.Reports.TemplateLayoutAnalysis.Models;
using KenticoInspector.Reports.TemplateLayoutAnalysis.Models.Data;
using NUnit.Framework;

namespace KenticoInspector.Reports.Tests
{
    [TestFixture(10)]
    [TestFixture(11)]
    [TestFixture(12)]
    public class TemplateLayoutAnalysisTests : AbstractReportTest<Report, Terms>
    {
        private readonly Report _mockReport;

        private IList<CmsPageTemplate> PageTemplatesWithoutIssues => new List<CmsPageTemplate>
        {
            new CmsPageTemplate
            {
                PageTemplateCodeName = "codename1",
                PageTemplateLayout = "<!-- Container --> <cms:CMSWebPartZone runat=\"server\" ZoneID=\"zoneMain\" /> "
            },
            new CmsPageTemplate
            {
                PageTemplateCodeName = "codename2",
                PageTemplateLayout = "<cms:CMSWebPartZone ZoneID=\"zoneA\" runat=\"server\" />"
            },
            new CmsPageTemplate
            {
                PageTemplateCodeName = "codename3",
                PageTemplateLayout = "<cms:CMSWebPartZone ZoneID=\"ZoneContent\" runat=\"server\" />"
            }
        };

        private IEnumerable<CmsPageTemplate> PageTemplatesWithIssues => new List<CmsPageTemplate>(PageTemplatesWithoutIssues)
        {
            new CmsPageTemplate
            {
                PageTemplateCodeName = "codename4",
                PageTemplateLayout = "<cms:CMSWebPartZone ZoneID=\"ZoneContent\" runat=\"server\" />"
            }
        };

        public TemplateLayoutAnalysisTests(int majorVersion) : base(majorVersion)
        {
            _mockReport = new Report(_mockDatabaseService.Object, _mockReportMetadataService.Object);
        }

        [Test]
        public void Should_ReturnGoodResult_When_PageTemplatesWithoutIssues()
        {
            // Arrange
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<CmsPageTemplate>(Scripts.GetCmsPageTemplates))
                .Returns(PageTemplatesWithoutIssues);

            // Act
            var results = _mockReport.GetResults();

            // Assert
            Assert.That(results.Status, Is.EqualTo(ReportResultsStatus.Good));
        }

        [Test]
        public void Should_ReturnInformationResult_When_PageTemplatesWithIssues()
        {
            // Arrange
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<CmsPageTemplate>(Scripts.GetCmsPageTemplates))
                .Returns(PageTemplatesWithIssues);

            // Act
            var results = _mockReport.GetResults();

            // Assert
            Assert.That(results.Data.Rows.Count, Is.EqualTo(1));
            Assert.That(results.Status, Is.EqualTo(ReportResultsStatus.Information));
        }
    }
}