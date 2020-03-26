using System.Collections.Generic;
using System.Linq;

using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Models;
using KenticoInspector.Reports.TaskProcessingAnalysis;
using KenticoInspector.Reports.TaskProcessingAnalysis.Models;

using NUnit.Framework;

namespace KenticoInspector.Reports.Tests
{
    [TestFixture(10)]
    [TestFixture(11)]
    public class TaskProcessingAnalysisTests : AbstractReportTest<Report, Terms>
    {
        private Report _mockReport;

        public TaskProcessingAnalysisTests(int majorVersion) : base(majorVersion)
        {
            _mockReport = new Report(_mockDatabaseService.Object, _mockReportMetadataService.Object);
        }

        [Test]
        public void Should_ReturnGoodResult_When_TasksWithoutIssues()
        {
            // Arrange
            ArrangeDatabaseService();

            // Act
            var results = _mockReport.GetResults();

            // Assert
            Assert.That(results.Status, Is.EqualTo(ReportResultsStatus.Good));
        }

        [Test]
        public void Should_ReturnWarningResult_When_IntegrationBusTasksWithIssues()
        {
            // Arrange
            ArrangeDatabaseService(unprocessedIntegrationBusTasks: 1);

            // Act
            var results = _mockReport.GetResults();

            // Assert
            AssertThatResultsDataIncludesTaskTypeDetails(results.Data, _mockReport.Metadata.Terms.CountIntegrationBusTask);
            Assert.That(results.Status, Is.EqualTo(ReportResultsStatus.Warning));
        }

        [Test]
        public void Should_ReturnWarningResult_When_ScheduledTasksWithIssues()
        {
            // Arrange
            ArrangeDatabaseService(unprocessedScheduledTasks: 1);

            // Act
            var results = _mockReport.GetResults();

            // Assert
            AssertThatResultsDataIncludesTaskTypeDetails(results.Data, _mockReport.Metadata.Terms.CountScheduledTask);
            Assert.That(results.Status, Is.EqualTo(ReportResultsStatus.Warning));
        }

        [Test]
        public void Should_ReturnWarningResult_When_SearchTasksWithIssues()
        {
            // Arrange
            ArrangeDatabaseService(unprocessedSearchTasks: 1);

            // Act
            var results = _mockReport.GetResults();

            // Assert
            AssertThatResultsDataIncludesTaskTypeDetails(results.Data, _mockReport.Metadata.Terms.CountSearchTask);
            Assert.That(results.Status, Is.EqualTo(ReportResultsStatus.Warning));
        }

        [Test]
        public void Should_ReturnWarningResult_When_StagingTasksWithIssues()
        {
            // Arrange
            ArrangeDatabaseService(unprocessedStagingTasks: 1);

            // Act
            var results = _mockReport.GetResults();

            // Assert
            AssertThatResultsDataIncludesTaskTypeDetails(results.Data, _mockReport.Metadata.Terms.CountStagingTask);
            Assert.That(results.Status, Is.EqualTo(ReportResultsStatus.Warning));
        }

        [Test]
        public void Should_ReturnWarningResult_When_WebFarmTasksWithIssues()
        {
            // Arrange
            ArrangeDatabaseService(unprocessedWebFarmTasks: 1);

            // Act
            var results = _mockReport.GetResults();

            // Assert
            AssertThatResultsDataIncludesTaskTypeDetails(results.Data, _mockReport.Metadata.Terms.CountWebFarmTask);
            Assert.That(results.Status, Is.EqualTo(ReportResultsStatus.Warning));
        }

        private static void AssertThatResultsDataIncludesTaskTypeDetails(dynamic data, Term term)
        {
            var resultsData = (IEnumerable<string>)data;
            var hasTasksListedInResults = resultsData.Any(x => x.Contains(term.ToString(), System.StringComparison.InvariantCultureIgnoreCase));

            Assert.That(hasTasksListedInResults, $"'{term}' not found in data.");
        }

        private void ArrangeDatabaseService(
            int unprocessedIntegrationBusTasks = 0,
            int unprocessedScheduledTasks = 0,
            int unprocessedSearchTasks = 0,
            int unprocessedStagingTasks = 0,
            int unprocessedWebFarmTasks = 0
        )
        {
            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFileScalar<int>(Scripts.GetIntegrationTasksCountInPast24Hours))
                .Returns(unprocessedIntegrationBusTasks);

            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFileScalar<int>(Scripts.GetCmsScheduledTasksCountInPast24Hours))
                .Returns(unprocessedScheduledTasks);

            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFileScalar<int>(Scripts.GetCmsSearchTasksCountInPast24Hours))
                .Returns(unprocessedSearchTasks);

            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFileScalar<int>(Scripts.GetStagingTasksCountInpast24Hours))
                .Returns(unprocessedStagingTasks);

            _mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFileScalar<int>(Scripts.GetCmsWebFarmTaskCountInPast24Hours))
                .Returns(unprocessedWebFarmTasks);
        }
    }
}