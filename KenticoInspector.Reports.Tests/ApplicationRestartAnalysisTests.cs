using System;
using System.Collections.Generic;

using KenticoInspector.Core.Constants;
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

        public ApplicationRestartAnalysisTests(int majorVersion) : base(majorVersion)
        {
            _mockReport = new Report(_mockDatabaseService.Object, _mockReportMetadataService.Object);
        }

        [Test]
        public void Should_ReturnEmptyResult_When_DatabaseHasNoEvents()
        {
            // Arrange
            var applicationRestartEvents = new List<CmsEventLog>();

            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<CmsEventLog>(Scripts.GetEventLogStartOrEndEvents))
                .Returns(applicationRestartEvents);

            // Act
            var results = _mockReport.GetResults();

            // Assert
            Assert.That(results.Type == ReportResultsType.Table);
            Assert.That(results.Data.Rows.Count == 0);
            Assert.That(results.Status == ReportResultsStatus.Information);
        }

        [Test]
        public void Should_ReturnResult_When_DatabaseHasEvents()
        {
            // Arrange
            var applicationRestartEvents = new List<CmsEventLog>
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

            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<CmsEventLog>(Scripts.GetEventLogStartOrEndEvents))
                .Returns(applicationRestartEvents);

            // Act
            var results = _mockReport.GetResults();

            // Assert
            Assert.That(results.Type == ReportResultsType.Table);
            Assert.That(results.Data.Rows.Count == 2);
            Assert.That(results.Status == ReportResultsStatus.Information);
        }
    }
}