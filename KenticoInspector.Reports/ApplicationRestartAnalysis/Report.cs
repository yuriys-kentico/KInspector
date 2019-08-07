using System;
using System.Collections.Generic;
using System.Linq;

using KenticoInspector.Core;
using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Helpers;
using KenticoInspector.Core.Models;
using KenticoInspector.Core.Services.Interfaces;
using KenticoInspector.Reports.ApplicationRestartAnalysis.Models;
using KenticoInspector.Reports.ApplicationRestartAnalysis.Models.Data;

namespace KenticoInspector.Reports.ApplicationRestartAnalysis
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
            ReportTags.EventLog,
            ReportTags.Health
        };

        public override ReportResults GetResults()
        {
            var eventLogs = databaseService.ExecuteSqlFromFile<CmsEventLog>(Scripts.GetEventLogStartOrEndEvents);

            return CompileResults(eventLogs);
        }

        private ReportResults CompileResults(IEnumerable<CmsEventLog> eventLogs)
        {
            if (!eventLogs.Any())
            {
                return new ReportResults
                {
                    Status = ReportResultsStatus.Good,
                    Summary = Metadata.Terms.GoodSummary,
                };
            }

            var totalEvents = eventLogs.Count();

            var totalStartEvents = eventLogs
                .Where(e => e.EventCode == "STARTAPP")
                .Count();

            var totalEndEvents = eventLogs
                .Where(e => e.EventCode == "ENDAPP")
                .Count();

            var earliestTime = totalEvents > 0 ? eventLogs.Min(e => e.EventTime) : new DateTime();
            var latestTime = totalEvents > 0 ? eventLogs.Max(e => e.EventTime) : new DateTime();

            var data = new TableResult<CmsEventLog>()
            {
                Name = Metadata.Terms.TableNames.ApplicationRestartEvents,
                Rows = eventLogs
            };

            return new ReportResults
            {
                Status = ReportResultsStatus.Information,
                Summary = Metadata.Terms.InformationSummary.With(new { totalEvents, totalStartEvents, totalEndEvents, earliestTime, latestTime }),
                Type = ReportResultsType.Table,
                Data = data
            };
        }
    }
}