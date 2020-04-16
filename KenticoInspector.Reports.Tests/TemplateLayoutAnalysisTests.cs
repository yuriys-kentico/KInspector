using System.Collections.Generic;
using System.Linq;

using KenticoInspector.Core.Modules.Models.Results;
using KenticoInspector.Core.Modules.Models.Results.Data;
using KenticoInspector.Reports.TemplateLayoutAnalysis;
using KenticoInspector.Reports.TemplateLayoutAnalysis.Models;
using KenticoInspector.Reports.TemplateLayoutAnalysis.Models.Data;
using KenticoInspector.Reports.TemplateLayoutAnalysis.Models.Results;
using KenticoInspector.Reports.Tests.AbstractClasses;

using NUnit.Framework;

namespace KenticoInspector.Reports.Tests
{
    [TestFixture(10)]
    [TestFixture(11)]
    [TestFixture(12)]
    public class TemplateLayoutAnalysisTests : AbstractReportTests<Report, Terms>
    {
        private readonly Report mockReport;

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

        private IEnumerable<CmsPageTemplate> PageTemplatesWithIssues =>
            new List<CmsPageTemplate>(PageTemplatesWithoutIssues)
            {
                new CmsPageTemplate
                {
                    PageTemplateCodeName = "codename4",
                    PageTemplateLayout = "<cms:CMSWebPartZone ZoneID=\"ZoneContent\" runat=\"server\" />"
                }
            };

        public TemplateLayoutAnalysisTests(int majorVersion) : base(majorVersion)
        {
            mockReport = ArrangeProperties(new Report(mockDatabaseService.Object));
        }

        [Test]
        public void Should_ReturnGoodResult_When_PageTemplatesWithoutIssues()
        {
            // Arrange
            mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<CmsPageTemplate>(Scripts.GetCmsPageTemplates))
                .Returns(PageTemplatesWithoutIssues);

            // Act
            var results = mockReport.GetResults();

            // Assert
            Assert.That(
                results.Status,
                Is.EqualTo(ResultsStatus.Good)
                );
        }

        [Test]
        public void Should_ReturnInformationResult_When_PageTemplatesWithIssues()
        {
            // Arrange
            mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<CmsPageTemplate>(Scripts.GetCmsPageTemplates))
                .Returns(PageTemplatesWithIssues);

            // Act
            var results = mockReport.GetResults();

            // Assert
            Assert.That(
                results.Data.First<TableResult<IdenticalPageTemplateResult>>()
                    .Rows.Count(),
                Is.EqualTo(1)
                );

            Assert.That(
                results.Status,
                Is.EqualTo(ResultsStatus.Information)
                );
        }
    }
}