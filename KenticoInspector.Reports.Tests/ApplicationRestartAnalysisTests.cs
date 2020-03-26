using System;
using System.Collections.Generic;
using System.Linq;

using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Models.Results;
using KenticoInspector.Reports.ApplicationRestartAnalysis;
using KenticoInspector.Reports.ApplicationRestartAnalysis.Models;
using KenticoInspector.Reports.ApplicationRestartAnalysis.Models.Data;

using NUnit.Framework;

namespace KenticoInspector.Reports.Tests
{
    [TestFixture(10)]
    [TestFixture(11)]
    [TestFixture(12)]
    public class ApplicationRestartAnalysisTests : AbstractReportTest<Report, Terms>
    {
        private Report _mockReport;

        private IEnumerable<CmsEventLog> CmsEventsWithoutStartAndEndCodes => new List<CmsEventLog>();

        private IEnumerable<CmsEventLog> CmsEventsWithStartAndEndCodes => new List<CmsEventLog>
        {
            new CmsEventLog
            {
                EventCode = "STARTAPP",
                EventTime = DateTime.Now.AddHours(-1),
                EventMachineName = "Server-01"
            },

            new CmsEventLog
            {
                EventCode = "ENDAPP",
                EventTime = DateTime.Now.AddHours(-1).AddMinutes(-1),
                EventMachineName = "Server-01"
            }
        };

        public ApplicationRestartAnalysisTests(int majorVersion) : base(majorVersion)
        {
            _mockReport = new Report(_mockDatabaseService.Object, _mockReportMetadataService.Object);
        }

        [TestCase(Category = "No events", TestName = "Database without events produces a good result")]
        public void Should_ReturnGoodResult_When_DatabaseWithoutEvents()
        {
            // Arrange
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<CmsEventLog>(Scripts.GetCmsEventLogsWithStartOrEndCode))
                .Returns(CmsEventsWithoutStartAndEndCodes);

            // Act
            var results = _mockReport.GetResults();

            // Assert
            Assert.That(results.Status, Is.EqualTo(ReportResultsStatus.Good));
        }

        [TestCase(Category = "One restart event", TestName = "Database with events produces an information result and lists two events")]
        public void Should_ReturnResult_When_DatabaseWithRestartEvents()
        {
            // Arrange
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<CmsEventLog>(Scripts.GetCmsEventLogsWithStartOrEndCode))
                .Returns(CmsEventsWithStartAndEndCodes);

            // Act
            var results = _mockReport.GetResults();

            // Assert
            Assert.That(results.Data.First<TableResult<CmsEventLog>>().Rows.Count(), Is.EqualTo(2));
            Assert.That(results.Status, Is.EqualTo(ReportResultsStatus.Information));
        }
    }
}