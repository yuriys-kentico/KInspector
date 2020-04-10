using System.Data;

using KenticoInspector.Core.Modules.Models.Results;
using KenticoInspector.Reports.DatabaseConsistencyCheck;
using KenticoInspector.Reports.DatabaseConsistencyCheck.Models;

using NUnit.Framework;

namespace KenticoInspector.Reports.Tests
{
    [TestFixture(10)]
    [TestFixture(11)]
    [TestFixture(12)]
    public class DatabaseConsistencyCheckTests : AbstractReportTests<Report, Terms>
    {
        private readonly Report mockReport;

        public DatabaseConsistencyCheckTests(int majorVersion) : base(majorVersion)
        {
            mockReport = ArrangeProperties(new Report(mockDatabaseService.Object));
        }

        [Test]
        public void Should_ReturnGoodResult_When_ResultsWithoutIssues()
        {
            // Arrange
            var emptyResult = new DataTable();
#pragma warning disable 0618 // This is a special exemption as the results of CheckDB are unknown
            mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFileAsDataTable(Scripts.GetCheckDbResults))
                .Returns(emptyResult);
#pragma warning restore 0618
            // Act
            var results = mockReport.GetResults();

            //Assert
            Assert.That(results.Status, Is.EqualTo(ResultsStatus.Good));
        }

        [Test]
        public void Should_ReturnErrorResult_When_ResultsWithIssues()
        {
            // Arrange
            var result = new DataTable();
            result.Columns.Add("TestColumn");
            result.Rows.Add("value");

# pragma warning disable 0618 // This is a special exemption as the results of CheckDB are unknown
            mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFileAsDataTable(Scripts.GetCheckDbResults))
                .Returns(result);
# pragma warning restore 0618

            // Act
            var results = mockReport.GetResults();

            //Assert
            Assert.That(results.Status, Is.EqualTo(ResultsStatus.Error));
        }
    }
}