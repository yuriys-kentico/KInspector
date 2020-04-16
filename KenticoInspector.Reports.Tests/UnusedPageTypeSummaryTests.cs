using System.Collections.Generic;
using System.Linq;

using KenticoInspector.Core.Modules.Models.Results;
using KenticoInspector.Core.Modules.Models.Results.Data;
using KenticoInspector.Reports.Tests.AbstractClasses;
using KenticoInspector.Reports.UnusedPageTypeSummary;
using KenticoInspector.Reports.UnusedPageTypeSummary.Models;
using KenticoInspector.Reports.UnusedPageTypeSummary.Models.Data;

using NUnit.Framework;

namespace KenticoInspector.Reports.Tests
{
    [TestFixture(10)]
    [TestFixture(11)]
    [TestFixture(12)]
    public class UnusedPageTypeSummaryTests : AbstractReportTests<Report, Terms>
    {
        private readonly Report mockReport;

        public IEnumerable<CmsClass> ClassesWithoutIssues => new List<CmsClass>();

        public IEnumerable<CmsClass> ClassesWithIssues => new List<CmsClass>
        {
            new CmsClass
            {
                ClassDisplayName = "Blog",
                ClassName = "CMS.Blog"
            },
            new CmsClass
            {
                ClassDisplayName = "Blog month",
                ClassName = "CMS.BlogMonth"
            },
            new CmsClass
            {
                ClassDisplayName = "Blog post",
                ClassName = "CMS.BlogPost"
            },
            new CmsClass
            {
                ClassDisplayName = "Chat - Transformation",
                ClassName = "Chat.Transformations"
            },
            new CmsClass
            {
                ClassDisplayName = "Dancing Goat site - Transformations",
                ClassName = "DancingGoat.Transformations"
            },
            new CmsClass
            {
                ClassDisplayName = "E-commerce - Transformations",
                ClassName = "Ecommerce.Transformations"
            }
        };

        public UnusedPageTypeSummaryTests(int majorVersion) : base(majorVersion)
        {
            mockReport = ArrangeProperties(new Report(mockDatabaseService.Object));
        }

        [Test]
        public void Should_ReturnInformationResult_When_ClassesWithIssues()
        {
            // Arrange
            mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<CmsClass>(Scripts.GetCmsClassNotInViewCmsTreeJoined))
                .Returns(ClassesWithIssues);

            // Act
            var results = mockReport.GetResults();

            // Assert
            Assert.That(
                results.Data.First<TableResult<CmsClass>>()
                    .Rows.Count(),
                Is.EqualTo(6)
                );

            Assert.That(
                results.Status,
                Is.EqualTo(ResultsStatus.Information)
                );
        }

        [Test]
        public void Should_ReturnInformationResult_When_ClassesWithoutIssues()
        {
            // Arrange
            mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<CmsClass>(Scripts.GetCmsClassNotInViewCmsTreeJoined))
                .Returns(ClassesWithoutIssues);

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