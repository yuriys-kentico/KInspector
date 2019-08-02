using System;
using System.Collections.Generic;

using KenticoInspector.Core;
using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Helpers;
using KenticoInspector.Core.Models;
using KenticoInspector.Core.Services.Interfaces;
using KenticoInspector.Reports.DatabaseTableSizeAnalysis.Data;
using KenticoInspector.Reports.DatabaseTableSizeAnalysis.Models;

namespace KenticoInspector.Reports.DatabaseTableSizeAnalysis
{
    public class Report : AbstractReport<Terms>
    {
        private readonly IDatabaseService databaseService;

        public Report(IDatabaseService databaseService, IReportMetadataService reportMetadataService) : base(reportMetadataService)
        {
            this.databaseService = databaseService;
        }

        public override IList<Version> CompatibleVersions => VersionHelper.GetVersionList("10", "11", "12");

        public override IList<string> Tags => new List<string>
        {
            ReportTags.Health
        };

        public override ReportResults GetResults()
        {
            var top25LargestTables = databaseService.ExecuteSqlFromFile<DatabaseTableSize>(Scripts.GetTop25LargestTables);

            return CompileResults(top25LargestTables);
        }

        private ReportResults CompileResults(IEnumerable<DatabaseTableSize> top25LargestTables)
        {
            var databaseTableSizeResult = new TableResult<DatabaseTableSize>
            {
                Name = Metadata.Terms.TableNames.Top25Results,
                Rows = top25LargestTables
            };

            return new ReportResults
            {
                Type = ReportResultsType.Table,
                Status = ReportResultsStatus.Information,
                Summary = Metadata.Terms.InformationSummary,
                Data = databaseTableSizeResult
            };
        }
    }
}