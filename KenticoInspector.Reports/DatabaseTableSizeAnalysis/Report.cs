using System.Collections.Generic;

using KenticoInspector.Core;
using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Models.Results;
using KenticoInspector.Core.Services;
using KenticoInspector.Reports.DatabaseTableSizeAnalysis.Data;
using KenticoInspector.Reports.DatabaseTableSizeAnalysis.Models;

namespace KenticoInspector.Reports.DatabaseTableSizeAnalysis
{
    public class Report : AbstractReport<Terms>
    {
        private readonly IDatabaseService databaseService;

        public Report(IDatabaseService databaseService)
        {
            this.databaseService = databaseService;
        }

        public override IList<string> Tags => new List<string>
        {
            ReportTags.Health
        };

        [SupportsVersions("10 - 12.0")]
        public override ReportResults GetResults()
        {
            var top25LargestTables = databaseService.ExecuteSqlFromFile<DatabaseTableSize>(Scripts.GetTop25LargestTables);

            return CompileResults(top25LargestTables);
        }

        private ReportResults CompileResults(IEnumerable<DatabaseTableSize> top25LargestTables)
        {
            return new ReportResults(ResultsStatus.Information)
            {
                Summary = Metadata.Terms.InformationSummary,
                Data = top25LargestTables.AsResult().WithLabel(Metadata.Terms.TableNames.Top25Results)
            };
        }
    }
}