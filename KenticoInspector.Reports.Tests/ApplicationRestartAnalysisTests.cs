using System;
using System.Collections.Generic;
using System.Linq;

using KenticoInspector.Core.Modules.Models.Results;
using KenticoInspector.Core.Modules.Models.Results.Data;
using KenticoInspector.Reports.ApplicationRestartAnalysis;
using KenticoInspector.Reports.ApplicationRestartAnalysis.Models;
using KenticoInspector.Reports.ApplicationRestartAnalysis.Models.Data;
using KenticoInspector.Reports.Tests.AbstractClasses;

using NUnit.Framework;

namespace KenticoInspector.Reports.Tests
{
    [TestFixture(10)]
    [TestFixture(11)]
    [TestFixture(12)]
    public class ApplicationRestartAnalysisTests : AbstractReportTests<Report, Terms>
    {
        private readonly Report mockReport;

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
                EventTime = DateTime.Now.AddHours(-1)
                    .AddMinutes(-1),
                EventMachineName = "Server-01"
            }
        };

        public ApplicationRestartAnalysisTests(int majorVersion) : base(majorVersion)
        {
            mockReport = ArrangeProperties(new Report(mockDatabaseService.Object));
        }

        [TestCase(
            Category = "No events",
            TestName = "Database without events produces a good result"
            )]
        public void Should_ReturnGoodResult_When_DatabaseWithoutEvents()
        {
            // Arrange
            mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<CmsEventLog>(Scripts.GetCmsEventLogsWithStartOrEndCode))
                .Returns(CmsEventsWithoutStartAndEndCodes);

            // Act
            var results = mockReport.GetResults();

            // Assert
            Assert.That(
                results.Status,
                Is.EqualTo(ResultsStatus.Good)
                );
        }

        [TestCase(
            Category = "One restart event",
            TestName = "Database with events produces an information result and lists two events"
            )]
        public void Should_ReturnResult_When_DatabaseWithRestartEvents()
        {
            // Arrange
            mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFile<CmsEventLog>(Scripts.GetCmsEventLogsWithStartOrEndCode))
                .Returns(CmsEventsWithStartAndEndCodes);

            // Act
            var results = mockReport.GetResults();

            // Assert
            Assert.That(
                results.Data.First<TableResult<CmsEventLog>>()
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