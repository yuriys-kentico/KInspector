using System.Data;
using System.Linq;

using KenticoInspector.Core.Instances.Services;
using KenticoInspector.Core.Modules;
using KenticoInspector.Core.Modules.Models.Results;
using KenticoInspector.Core.Modules.Models.Results.Data;
using KenticoInspector.Modules;
using KenticoInspector.Reports.DatabaseConsistencyCheck.Models;

using static KenticoInspector.Core.Modules.Models.Tags;

namespace KenticoInspector.Reports.DatabaseConsistencyCheck
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
#pragma warning disable 0618 // This is a special exemption as the results of CheckDB are unknown
            var checkDbResults = databaseService.ExecuteSqlFromFileAsDataTable(Scripts.GetCheckDbResults);
#pragma warning restore 0618

            return CompileResults(checkDbResults);
        }

        private ReportResults CompileResults(DataTable checkDbResults)
        {
            var hasNoIssues = checkDbResults.Rows.Count == 0;

            if (hasNoIssues)
                return new ReportResults(ResultsStatus.Good)
                {
                    Summary = Metadata.Terms.GoodSummary
                };

            return new ReportResults(ResultsStatus.Error)
            {
                Summary = Metadata.Terms.ErrorSummary,
                Data = checkDbResults.Rows.OfType<DataRow>()
                    .AsResult()
            };
        }
    }
}