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
    public class DatabaseTableSizeAnalysisTest : AbstractReportTest<Report, Terms>
    {
        private Report _mockReport;

        public DatabaseTableSizeAnalysisTest(int majorVersion) : base(majorVersion)
        {
            _mockReport = new Report(_mockDatabaseService.Object, _mockReportMetadataService.Object);
        }

        [Test]
        public void Should_ReturnInformationStatus()
        {
            // Arrange
            IEnumerable<DatabaseTableSize> dbResults = GetCleanResults();
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<DatabaseTableSize>(Scripts.GetTop25LargestTables))
                .Returns(dbResults);

            // Act
            var results = _mockReport.GetResults();

            // Assert
            Assert.That(results.Data.Rows.Count == 25);
            Assert.That(results.Status == ReportResultsStatus.Information);
        }

        private List<DatabaseTableSize> GetCleanResults()
        {
            var results = new List<DatabaseTableSize>();

            for (var i = 0; i < 25; i++)
            {
                results.Add(new DatabaseTableSize() { TableName = $"table {i}", Rows = i, BytesPerRow = i, SizeInMB = i });
            }

            return results;
        }
    }
}