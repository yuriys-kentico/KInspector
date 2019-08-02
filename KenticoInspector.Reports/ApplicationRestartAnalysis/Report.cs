using KenticoInspector.Core;
using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Helpers;
using KenticoInspector.Core.Models;
using KenticoInspector.Core.Services.Interfaces;
using KenticoInspector.Reports.ApplicationRestartAnalysis.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KenticoInspector.Reports.ApplicationRestartAnalysis
{
    public class Report : AbstractReport<Terms>
    {
        private readonly IDatabaseService databaseService;

        public Report(IDatabaseService databaseService, IReportMetadataService reportMetadataService) : base(reportMetadataService)
        {
            this.databaseService = databaseService;
        }

        public override IList<Version> CompatibleVersions => VersionHelper.GetVersionList("10", "11");

        public override IList<string> Tags => new List<string>
        {
            ReportTags.EventLog,
            ReportTags.Health
        };

        public override ReportResults GetResults()
        {
            var eventLogs = databaseService.ExecuteSqlFromFile<CmsEventLog>(Scripts.GetEventLog);

            return CompileResults(eventLogs);
        }

        private ReportResults CompileResults(IEnumerable<CmsEventLog> eventLogs)
        {
            var totalEvents = eventLogs.Count();

            var totalStartEvents = eventLogs
                .Where(e => e.EventCode == "STARTAPP")
                .Count();

            var totalEndEvents = eventLogs
                .Where(e => e.EventCode == "ENDAPP")
                .Count();

            var earliestTime = totalEvents > 0 ? eventLogs.Min(e => e.EventTime) : new DateTime();
            var latestTime = totalEvents > 0 ? eventLogs.Max(e => e.EventTime) : new DateTime();

            var totalEventsText = Metadata.Terms.CountTotalEvent.With(new { totalEvents });

            var totalStartEventsText = Metadata.Terms.CountStartEvent.With(new { totalStartEvents });

            var totalEndEventsText = Metadata.Terms.CountEndEvent.With(new { totalEndEvents });

            string timeSpanText = string.Empty;

            if (earliestTime.Year > 1)
            {
                timeSpanText = Metadata.Terms.SpanningEarliestLatest.With(new { earliestTime, latestTime });
            }

            var data = new TableResult<dynamic>()
            {
                Name = Metadata.Terms.ApplicationRestartEvents,
                Rows = eventLogs
            };

            return new ReportResults
            {
                Type = ReportResultsType.Table,
                Status = ReportResultsStatus.Information,
                Summary = $"{totalEventsText} ({totalStartEventsText}, {totalEndEventsText}) {timeSpanText}",
                Data = data
            };
        }
    }
}