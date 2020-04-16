using System.Collections.Generic;
using System.Linq;

using KenticoInspector.Core.Instances.Services;
using KenticoInspector.Core.Modules;
using KenticoInspector.Core.Modules.Models.Results;
using KenticoInspector.Core.Modules.Models.Results.Data;
using KenticoInspector.Modules;
using KenticoInspector.Reports.TaskProcessingAnalysis.Models;
using KenticoInspector.Reports.TaskProcessingAnalysis.Models.Data;

using static KenticoInspector.Core.Modules.Models.Tags;

namespace KenticoInspector.Reports.TaskProcessingAnalysis
{
    public class Report : AbstractReport<Terms>
    {
        private readonly IDatabaseService databaseService;

        public Report(IDatabaseService databaseService)
        {
            this.databaseService = databaseService;
        }

        [Tags(Health)]
        [SupportsVersions("10 - 12.0")]
        public override ReportResults GetResults()
        {
            var scheduledTasks =
                databaseService.ExecuteSqlFromFile<CmsScheduledTask>(Scripts.GetCmsScheduledTasksInPast24Hours);

            var searchTasks = databaseService.ExecuteSqlFromFile<CmsSearchTask>(Scripts.GetCmsSearchTasksInPast24Hours);

            var integrationTasks =
                databaseService.ExecuteSqlFromFile<CmsIntegrationTask>(Scripts.GetCmsIntegrationTasksInPast24Hours);

            var stagingTasks =
                databaseService.ExecuteSqlFromFile<CmsStagingTask>(Scripts.GetCmsStagingTasksInpast24Hours);

            var webFarmTasks =
                databaseService.ExecuteSqlFromFile<CmsWebFarmTask>(Scripts.GetCmsWebFarmTasksInPast24Hours);

            return CompileResults(
                scheduledTasks,
                searchTasks,
                integrationTasks,
                stagingTasks,
                webFarmTasks
                );
        }

        private ReportResults CompileResults(
            IEnumerable<CmsScheduledTask> scheduledTasks,
            IEnumerable<CmsSearchTask> searchTasks,
            IEnumerable<CmsIntegrationTask> integrationTasks,
            IEnumerable<CmsStagingTask> stagingTasks,
            IEnumerable<CmsWebFarmTask> webFarmTasks
            )
        {
            var totalCount = scheduledTasks.Count()
                + searchTasks.Count()
                + integrationTasks.Count()
                + stagingTasks.Count()
                + webFarmTasks.Count();

            if (totalCount == 0)
                return new ReportResults(ResultsStatus.Good)
                {
                    Summary = Metadata.Terms.Summaries.Good
                };

            return new ReportResults(ResultsStatus.Warning)
            {
                Summary = Metadata.Terms.Summaries.Warning.With(
                    new
                    {
                        totalCount
                    }
                    ),
                Data =
                {
                    scheduledTasks.AsResult()
                        .WithLabel(Metadata.Terms.TableTitles.ScheduledTasks),
                    searchTasks.AsResult()
                        .WithLabel(Metadata.Terms.TableTitles.SearchTasks),
                    integrationTasks.AsResult()
                        .WithLabel(Metadata.Terms.TableTitles.IntegrationBusTasks),
                    stagingTasks.AsResult()
                        .WithLabel(Metadata.Terms.TableTitles.StagingTasks),
                    webFarmTasks.AsResult()
                        .WithLabel(Metadata.Terms.TableTitles.WebFarmTasks)
                }
            };
        }
    }
}