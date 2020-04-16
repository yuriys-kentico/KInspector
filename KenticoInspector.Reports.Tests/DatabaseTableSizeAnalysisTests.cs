using System.Collections.Generic;
using System.Linq;

using KenticoInspector.Core.Modules.Models.Results;
using KenticoInspector.Core.Modules.Models.Results.Data;
using KenticoInspector.Reports.DatabaseTableSizeAnalysis;
using KenticoInspector.Reports.DatabaseTableSizeAnalysis.Models;
using KenticoInspector.Reports.DatabaseTableSizeAnalysis.Models.Data;
using KenticoInspector.Reports.Tests.AbstractClasses;

using NUnit.Framework;

namespace KenticoInspector.Reports.Tests
{
    [TestFixture(10)]
    [TestFixture(11)]
    [TestFixture(12)]
    public class DatabaseTableSizeAnalysisTests : AbstractReportTests<Report, Terms>
    {
        private readonly Report mockReport;

        public DatabaseTableSizeAnalysisTests(int majorVersion) : base(majorVersion)
        {
            mockReport = ArrangeProperties(new Report(mockDatabaseService.Object));
        }

        private List<DatabaseTableSize> GetCleanResults(int count)
        {
            var results = new List<DatabaseTableSize>();

            for (var i = 0; i < count; i++)
                results.Add(
                    new DatabaseTableSize
                    {
                        TableName = $"table {i}",
                        Rows = i,
                        BytesPerRow = i,
                        SizeInMB = i
                    }
                    );

            return results;
        }

        [Test]
        public void Should_ReturnInformationResult()
        {
            // Arrange
            IEnumerable<DatabaseTableSize> databaseTableSites = GetCleanResults(25);

            mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<DatabaseTableSize>(Scripts.GetTop25LargestTables))
                .Returns(databaseTableSites);

            // Act
            var results = mockReport.GetResults();

            // Assert
            Assert.That(
                results.Data.First<TableResult<DatabaseTableSize>>()
                    .Rows.Count(),
                Is.EqualTo(25)
                );

            Assert.That(
                results.Status,
                Is.EqualTo(ResultsStatus.Information)
                );
        }
    }
}