using System.Collections.Generic;
using System.Linq;

using KenticoInspector.Core.Modules.Models.Results;
using KenticoInspector.Core.Modules.Models.Results.Data;
using KenticoInspector.Reports.PageTypeFieldAnalysis;
using KenticoInspector.Reports.PageTypeFieldAnalysis.Models;
using KenticoInspector.Reports.PageTypeFieldAnalysis.Models.Data;
using KenticoInspector.Reports.Tests.AbstractClasses;

using NUnit.Framework;

namespace KenticoInspector.Reports.Tests
{
    [TestFixture(10)]
    [TestFixture(11)]
    [TestFixture(12)]
    public class PageTypeFieldAnalysisTests : AbstractReportTests<Report, Terms>
    {
        private readonly Report mockReport;

        private List<CmsPageTypeField> CmsPageTypeFieldsWithoutIssues => new List<CmsPageTypeField>();

        private List<CmsPageTypeField> CmsPageTypeFieldsWithIdenticalNamesAndDifferentDataTypes =>
            new List<CmsPageTypeField>
            {
                new CmsPageTypeField
                {
                    PageTypeCodeName = "DancingGoatMvc.Article",
                    FieldName = "ArticleText",
                    FieldDataType = "varchar"
                },
                new CmsPageTypeField
                {
                    PageTypeCodeName = "DancingGoatMvc.AboutUs",
                    FieldName = "ArticleText",
                    FieldDataType = "int"
                }
            };

        public PageTypeFieldAnalysisTests(int majorVersion) : base(majorVersion)
        {
            mockReport = ArrangeProperties(new Report(mockDatabaseService.Object));
        }

        [TestCase(
            Category = "Matching fields have save data types",
            TestName = "Page type fields with matching names and data types produce a good result"
            )]
        public void Should_ReturnGoodResult_When_FieldsHaveNoIssues()
        {
            // Arrange
            mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<CmsPageTypeField>(Scripts.GetCmsPageTypeFields))
                .Returns(CmsPageTypeFieldsWithoutIssues);

            // Act
            var results = mockReport.GetResults();

            // Assert
            Assert.That(
                results.Status,
                Is.EqualTo(ResultsStatus.Good)
                );
        }

        [TestCase(
            Category = "Matching fields have different data types",
            TestName = "Page type fields with matching names and different data types produce an information result"
            )]
        public void Should_ReturnInformationResult_When_FieldsWithMatchingNamesHaveDifferentDataTypes()
        {
            // Arrange
            mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<CmsPageTypeField>(Scripts.GetCmsPageTypeFields))
                .Returns(CmsPageTypeFieldsWithIdenticalNamesAndDifferentDataTypes);

            // Act
            var results = mockReport.GetResults();

            // Assert
            Assert.That(
                results.Data.First<TableResult<CmsPageTypeField>>()
                    .Rows.Count(),
                Is.EqualTo(2)
                );

            Assert.That(
                results.Status,
                Is.EqualTo(ResultsStatus.Information)
                );
        }
    }
}