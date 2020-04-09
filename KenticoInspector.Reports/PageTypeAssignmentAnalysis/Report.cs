using System.Collections.Generic;
using System.Linq;

using KenticoInspector.Core;
using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Models.Results;
using KenticoInspector.Core.Services;
using KenticoInspector.Infrastructure;
using KenticoInspector.Reports.PageTypeAssignmentAnalysis.Models;
using KenticoInspector.Reports.PageTypeAssignmentAnalysis.Models.Data;

using static KenticoInspector.Core.Models.Tags;

namespace KenticoInspector.Reports.PageTypeAssignmentAnalysis
{
    public class Report : AbstractReport<Terms>
    {
        private readonly IDatabaseService databaseService;

        public Report(IDatabaseService databaseService)
        {
            this.databaseService = databaseService;
        }

        [Tags(ContentModeling, Health)]
        [SupportsVersions("10 - 12.0")]
        public override ReportResults GetResults()
        {
            var pageTypesNotAssignedToSite = databaseService.ExecuteSqlFromFile<CmsPageType>(Scripts.GetPageTypesNotAssignedToSite);

            return CompileResults(pageTypesNotAssignedToSite);
        }

        private ReportResults CompileResults(IEnumerable<CmsPageType> pageTypesNotAssignedToSite)
        {
            if (!pageTypesNotAssignedToSite.Any())
            {
                return new ReportResults(ResultsStatus.Good)
                {
                    Summary = Metadata.Terms.GoodSummary
                };
            }

            var unassignedPageTypeCount = pageTypesNotAssignedToSite.Count();

            return new ReportResults(ResultsStatus.Warning)
            {
                Summary = Metadata.Terms.WarningSummary.With(new { unassignedPageTypeCount }),
                Data = pageTypesNotAssignedToSite.AsResult().WithLabel(Metadata.Terms.TableNames.UnassignedPageTypes)
            };
        }
    }
}