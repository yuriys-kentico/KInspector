using System.Collections.Generic;
using System.Linq;

using KenticoInspector.Core.Modules.Models.Results;
using KenticoInspector.Core.Modules.Models.Results.Data;
using KenticoInspector.Core.Tests.Mocks;
using KenticoInspector.Reports.TaskProcessingAnalysis;
using KenticoInspector.Reports.TaskProcessingAnalysis.Models;
using KenticoInspector.Reports.TaskProcessingAnalysis.Models.Data;

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

        private IEnumerable<CmsWebFarmTask> WebFarmTasksWithIssues => new List<CmsWebFarmTask>
        {
            new CmsWebFarmTask()
            {
                TaskID = 1
            }
        };

        private IEnumerable<CmsIntegrationTask> IntegrationTasksWithIssues => new List<CmsIntegrationTask>

        {
            new CmsIntegrationTask()
            {
                TaskID = 1
            }
        };

        private IEnumerable<CmsScheduledTask> ScheduledTasksWithIssues => new List<CmsScheduledTask>

        {
            new CmsScheduledTask
            {
                TaskID = 1
            }
        };

        private IEnumerable<CmsSearchTask> SearchTasksWithIssues => new List<CmsSearchTask>

        {
            new CmsSearchTask
            {
                SearchTaskID = 1
            }
        };

        private IEnumerable<CmsStagingTask> StagingTasksWithIssues => new List<CmsStagingTask>

        {
            new CmsStagingTask
            {
                TaskID = 1
            }
        };

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
        public void Should_ReturnWarningResult_When_ScheduledTasksWithIssues()
        {
            // Arrange
            ArrangeDatabaseService(scheduledTasks: true);

            // Act
            var results = mockReport.GetResults();

            // Assert
            Assert.That(results.Data.First<TableResult<CmsScheduledTask>>().Rows.Count(), Is.EqualTo(1));
            Assert.That(results.Status, Is.EqualTo(ResultsStatus.Warning));
        }

        [Test]
        public void Should_ReturnWarningResult_When_SearchTasksWithIssues()
        {
            // Arrange
            ArrangeDatabaseService(searchTasks: true);

            // Act
            var results = mockReport.GetResults();

            // Assert
            Assert.That(results.Data.First<TableResult<CmsSearchTask>>().Rows.Count(), Is.EqualTo(1));
            Assert.That(results.Status, Is.EqualTo(ResultsStatus.Warning));
        }

        [Test]
        public void Should_ReturnWarningResult_When_IntegrationBusTasksWithIssues()
        {
            // Arrange
            ArrangeDatabaseService(integrationBusTasks: true);

            // Act
            var results = mockReport.GetResults();

            // Assert
            Assert.That(results.Data.First<TableResult<CmsIntegrationTask>>().Rows.Count(), Is.EqualTo(1));
            Assert.That(results.Status, Is.EqualTo(ResultsStatus.Warning));
        }

        [Test]
        public void Should_ReturnWarningResult_When_StagingTasksWithIssues()
        {
            // Arrange
            ArrangeDatabaseService(stagingTasks: true);

            // Act
            var results = mockReport.GetResults();

            // Assert
            Assert.That(results.Data.First<TableResult<CmsStagingTask>>().Rows.Count(), Is.EqualTo(1));
            Assert.That(results.Status, Is.EqualTo(ResultsStatus.Warning));
        }

        [Test]
        public void Should_ReturnWarningResult_When_WebFarmTasksWithIssues()
        {
            // Arrange
            ArrangeDatabaseService(webFarmTasks: true);

            // Act
            var results = mockReport.GetResults();

            // Assert
            Assert.That(results.Data.First<TableResult<CmsWebFarmTask>>().Rows.Count(), Is.EqualTo(1));
            Assert.That(results.Status, Is.EqualTo(ResultsStatus.Warning));
        }

        private void ArrangeDatabaseService(
            bool scheduledTasks = false,
            bool searchTasks = false,
            bool integrationBusTasks = false,
            bool stagingTasks = false,
            bool webFarmTasks = false
        )
        {
            mockDatabaseService.SetupExecuteSqlFromFile(Scripts.GetCmsScheduledTasksInPast24Hours, scheduledTasks ? ScheduledTasksWithIssues : Enumerable.Empty<CmsScheduledTask>());
            mockDatabaseService.SetupExecuteSqlFromFile(Scripts.GetCmsSearchTasksInPast24Hours, searchTasks ? SearchTasksWithIssues : Enumerable.Empty<CmsSearchTask>());
            mockDatabaseService.SetupExecuteSqlFromFile(Scripts.GetCmsIntegrationTasksInPast24Hours, integrationBusTasks ? IntegrationTasksWithIssues : Enumerable.Empty<CmsIntegrationTask>());
            mockDatabaseService.SetupExecuteSqlFromFile(Scripts.GetCmsStagingTasksInpast24Hours, stagingTasks ? StagingTasksWithIssues : Enumerable.Empty<CmsStagingTask>());
            mockDatabaseService.SetupExecuteSqlFromFile(Scripts.GetCmsWebFarmTasksInPast24Hours, webFarmTasks ? WebFarmTasksWithIssues : Enumerable.Empty<CmsWebFarmTask>());
        }
    }
}