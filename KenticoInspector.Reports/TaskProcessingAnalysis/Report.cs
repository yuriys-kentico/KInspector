using System;
using System.Collections.Generic;
using System.Linq;

using KenticoInspector.Core;
using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Helpers;
using KenticoInspector.Core.Models.Results;
using KenticoInspector.Core.Services.Interfaces;
using KenticoInspector.Reports.TaskProcessingAnalysis.Models;
using KenticoInspector.Reports.TaskProcessingAnalysis.Models.Results;

namespace KenticoInspector.Reports.TaskProcessingAnalysis
{
    public class Report : AbstractReport<Terms>
    {
        private readonly IDatabaseService databaseService;

        public Report(IDatabaseService databaseService)
        {
            this.databaseService = databaseService;
        }

        public override IList<Version> CompatibleVersions => VersionHelper.GetVersionList("10", "11");

        public override IList<string> Tags => new List<string>
        {
           ReportTags.Health
        };

        public override ReportResults GetResults()
        {
            var unprocessedIntegrationBusTasks = databaseService.ExecuteSqlFromFileScalar<int>(Scripts.GetIntegrationTasksCountInPast24Hours);
            var unprocessedScheduledTasks = databaseService.ExecuteSqlFromFileScalar<int>(Scripts.GetCmsScheduledTasksCountInPast24Hours);
            var unprocessedSearchTasks = databaseService.ExecuteSqlFromFileScalar<int>(Scripts.GetCmsSearchTasksCountInPast24Hours);
            var unprocessedStagingTasks = databaseService.ExecuteSqlFromFileScalar<int>(Scripts.GetStagingTasksCountInpast24Hours);
            var unprocessedWebFarmTasks = databaseService.ExecuteSqlFromFileScalar<int>(Scripts.GetCmsWebFarmTaskCountInPast24Hours);

            var rawResults = new List<TaskCountResult>
            {
                new TaskCountResult(Metadata.Terms.CountIntegrationBusTask, unprocessedIntegrationBusTasks ),
                new TaskCountResult(Metadata.Terms.CountScheduledTask, unprocessedScheduledTasks ),
                new TaskCountResult(Metadata.Terms.CountSearchTask, unprocessedSearchTasks ),
                new TaskCountResult(Metadata.Terms.CountStagingTask, unprocessedStagingTasks ),
                new TaskCountResult(Metadata.Terms.CountWebFarmTask, unprocessedWebFarmTasks )
            };

            return CompileResults(rawResults);
        }

        private ReportResults CompileResults(IEnumerable<TaskCountResult> taskTypesAndCounts)
        {
            var count = taskTypesAndCounts.Sum(x => x.Count);

            if (count == 0)
            {
                return new ReportResults(ResultsStatus.Good)
                {
                    Summary = Metadata.Terms.GoodSummary
                };
            }

            var results = new ReportResults(ResultsStatus.Warning)
            {
                Summary = Metadata.Terms.WarningSummary.With(new { count })
            };

            var lines = taskTypesAndCounts
                .Where(taskTypeAndCount => taskTypeAndCount.Count > 0)
                .Select(taskTypeAndCount => taskTypeAndCount.Term.With(new { count = taskTypeAndCount.Count }));

            foreach (var result in lines)
            {
                results.Data.Add(result);
            }

            return results;
        }
    }
}