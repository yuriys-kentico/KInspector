using KenticoInspector.Core.Modules.Models.Results;
using KenticoInspector.Core.Modules.Models.Results.Data;
using KenticoInspector.Core.TokenExpressions.Models;
using KenticoInspector.Reports.TaskProcessingAnalysis;
using KenticoInspector.Reports.TaskProcessingAnalysis.Models;

using NUnit.Framework;

namespace KenticoInspector.Reports.Tests
{
    [TestFixture(10)]
    [TestFixture(11)]
    public class TaskProcessingAnalysisTests : AbstractReportTests<Report, Terms>
    {
        private readonly Report mockReport;

        public TaskProcessingAnalysisTests(int majorVersion) : base(majorVersion)
        {
            mockReport = ArrangeProperties(new Report(mockDatabaseService.Object));
        }

        [Test]
        public void Should_ReturnGoodResult_When_TasksWithoutIssues()
        {
            // Arrange
            ArrangeDatabaseService();

            // Act
            var results = mockReport.GetResults();

            // Assert
            Assert.That(results.Status, Is.EqualTo(ResultsStatus.Good));
        }

        [Test]
        public void Should_ReturnWarningResult_When_IntegrationBusTasksWithIssues()
        {
            // Arrange
            ArrangeDatabaseService(unprocessedIntegrationBusTasks: 1);

            // Act
            var results = mockReport.GetResults();

            // Assert
            AssertThatResultsDataIncludesTaskTypeDetails(results.Data, mockReport.Metadata.Terms.CountIntegrationBusTask);
            Assert.That(results.Status, Is.EqualTo(ResultsStatus.Warning));
        }

        [Test]
        public void Should_ReturnWarningResult_When_ScheduledTasksWithIssues()
        {
            // Arrange
            ArrangeDatabaseService(unprocessedScheduledTasks: 1);

            // Act
            var results = mockReport.GetResults();

            // Assert
            AssertThatResultsDataIncludesTaskTypeDetails(results.Data, mockReport.Metadata.Terms.CountScheduledTask);
            Assert.That(results.Status, Is.EqualTo(ResultsStatus.Warning));
        }

        [Test]
        public void Should_ReturnWarningResult_When_SearchTasksWithIssues()
        {
            // Arrange
            ArrangeDatabaseService(unprocessedSearchTasks: 1);

            // Act
            var results = mockReport.GetResults();

            // Assert
            AssertThatResultsDataIncludesTaskTypeDetails(results.Data, mockReport.Metadata.Terms.CountSearchTask);
            Assert.That(results.Status, Is.EqualTo(ResultsStatus.Warning));
        }

        [Test]
        public void Should_ReturnWarningResult_When_StagingTasksWithIssues()
        {
            // Arrange
            ArrangeDatabaseService(unprocessedStagingTasks: 1);

            // Act
            var results = mockReport.GetResults();

            // Assert
            AssertThatResultsDataIncludesTaskTypeDetails(results.Data, mockReport.Metadata.Terms.CountStagingTask);
            Assert.That(results.Status, Is.EqualTo(ResultsStatus.Warning));
        }

        [Test]
        public void Should_ReturnWarningResult_When_WebFarmTasksWithIssues()
        {
            // Arrange
            ArrangeDatabaseService(unprocessedWebFarmTasks: 1);

            // Act
            var results = mockReport.GetResults();

            // Assert
            AssertThatResultsDataIncludesTaskTypeDetails(results.Data, mockReport.Metadata.Terms.CountWebFarmTask);
            Assert.That(results.Status, Is.EqualTo(ResultsStatus.Warning));
        }

        private static void AssertThatResultsDataIncludesTaskTypeDetails(ReportResultsData data, Term term)
        {
            foreach (var result in data)
            {
                if (result is StringResult stringResult)
                {
                    Assert.That(stringResult.String, Does.Contain(term.ToString()));
                }
            }
        }

        private void ArrangeDatabaseService(
            int unprocessedIntegrationBusTasks = 0,
            int unprocessedScheduledTasks = 0,
            int unprocessedSearchTasks = 0,
            int unprocessedStagingTasks = 0,
            int unprocessedWebFarmTasks = 0
        )
        {
            mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFileScalar<int>(Scripts.GetIntegrationTasksCountInPast24Hours))
                .Returns(unprocessedIntegrationBusTasks);

            mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFileScalar<int>(Scripts.GetCmsScheduledTasksCountInPast24Hours))
                .Returns(unprocessedScheduledTasks);

            mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFileScalar<int>(Scripts.GetCmsSearchTasksCountInPast24Hours))
                .Returns(unprocessedSearchTasks);

            mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFileScalar<int>(Scripts.GetStagingTasksCountInpast24Hours))
                .Returns(unprocessedStagingTasks);

            mockDatabaseService
                .Setup(p => p.ExecuteSqlFromFileScalar<int>(Scripts.GetCmsWebFarmTaskCountInPast24Hours))
                .Returns(unprocessedWebFarmTasks);
        }
    }
}