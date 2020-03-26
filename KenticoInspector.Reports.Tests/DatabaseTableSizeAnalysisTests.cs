using System.Collections.Generic;

using KenticoInspector.Core.Constants;
using KenticoInspector.Reports.DatabaseTableSizeAnalysis;
using KenticoInspector.Reports.DatabaseTableSizeAnalysis.Data;
using KenticoInspector.Reports.DatabaseTableSizeAnalysis.Models;

using NUnit.Framework;

namespace KenticoInspector.Reports.Tests
{
    [TestFixture(10)]
    [TestFixture(11)]
    [TestFixture(12)]
    public class DatabaseTableSizeAnalysisTests : AbstractReportTest<Report, Terms>
    {
        private Report _mockReport;

        public DatabaseTableSizeAnalysisTests(int majorVersion) : base(majorVersion)
        {
            _mockReport = new Report(_mockDatabaseService.Object, _mockReportMetadataService.Object);
        }

        [Test]
        public void Should_ReturnInformationResult()
        {
            // Arrange
            IEnumerable<DatabaseTableSize> databaseTableSites = GetCleanResults(25);

            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<DatabaseTableSize>(Scripts.GetTop25LargestTables))
                .Returns(databaseTableSites);

            // Act
            var results = _mockReport.GetResults();

            // Assert
            Assert.That(results.Data.Rows.Count, Is.EqualTo(25));
            Assert.That(results.Status, Is.EqualTo(ReportResultsStatus.Information));
        }

        private List<DatabaseTableSize> GetCleanResults(int count)
        {
            var results = new List<DatabaseTableSize>();

            for (var i = 0; i < count; i++)
            {
                results.Add(new DatabaseTableSize()
                {
                    TableName = $"table {i}",
                    Rows = i,
                    BytesPerRow = i,
                    SizeInMB = i
                });
            }

            return results;
        }
    }
}