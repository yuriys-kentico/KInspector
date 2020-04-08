using System;
using System.Collections.Generic;
using System.Linq;

using KenticoInspector.Core;
using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Models.Results;
using KenticoInspector.Core.Services;
using KenticoInspector.Reports.ApplicationRestartAnalysis.Models;
using KenticoInspector.Reports.ApplicationRestartAnalysis.Models.Data;

namespace KenticoInspector.Reports.ApplicationRestartAnalysis
{
    public class Report : AbstractReport<Terms>
    {
        private readonly IDatabaseService databaseService;

        public override IList<string> Tags => new List<string>
        {
            ReportTags.EventLog,
            ReportTags.Health
        };

        public Report(
            IDatabaseService databaseService
            )
        {
            this.databaseService = databaseService;
        }

        [SupportsVersions("10 - 12.0")]
        public override ReportResults GetResults()
        {
            var cmsEventLogs = databaseService.ExecuteSqlFromFile<CmsEventLog>(Scripts.GetCmsEventLogsWithStartOrEndCode);

            return CompileResults(cmsEventLogs);
        }

        private ReportResults CompileResults(IEnumerable<CmsEventLog> cmsEventLogs)
        {
            if (!cmsEventLogs.Any())
            {
                return new ReportResults(ResultsStatus.Good)
                {
                    Summary = Metadata.Terms.Summaries.Good
                };
            }

            var totalEvents = cmsEventLogs.Count();

            var totalStartEvents = cmsEventLogs
                .Where(e => e.EventCode == "STARTAPP")
                .Count();

            var totalEndEvents = cmsEventLogs
                .Where(e => e.EventCode == "ENDAPP")
                .Count();

            var earliestTime = totalEvents > 0
                ? cmsEventLogs.Min(e => e.EventTime)
                : new DateTime();

            var latestTime = totalEvents > 0
                ? cmsEventLogs.Max(e => e.EventTime)
                : new DateTime();

            var summary = Metadata.Terms.Summaries.Information.With(new
            {
                earliestTime,
                latestTime,
                totalEndEvents,
                totalEvents,
                totalStartEvents
            });

            return new ReportResults(ResultsStatus.Information)
            {
                Summary = summary,
                Data = cmsEventLogs.AsResult().WithLabel(Metadata.Terms.TableTitles.ApplicationRestartEvents)
            };
        }
    }
}