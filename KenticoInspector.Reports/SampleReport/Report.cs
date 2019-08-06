using System;
using System.Collections.Generic;
using System.Text;

using KenticoInspector.Core;
using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Helpers;
using KenticoInspector.Core.Models;
using KenticoInspector.Core.Services.Interfaces;
using KenticoInspector.Reports.SampleReport.Models;

namespace KenticoInspector.Reports.SampleReport
{
    public class Report : AbstractReport<Terms>
    {
        private readonly IDatabaseService databaseService;

        private readonly Random random = new Random();

        public Report(IDatabaseService databaseService, IReportMetadataService reportMetadataService) : base(reportMetadataService)
        {
            this.databaseService = databaseService;
        }

        public override IList<Version> CompatibleVersions => VersionHelper.GetVersionList("10", "11", "12");

        public override IList<string> Tags => new List<string>
        {
            ReportTags.Consistency
        };

        public override ReportResults GetResults()
        {
            var issueCount = random.Next(0, 3);

            var data = new List<string>();

            for (int i = 0; i < issueCount; i++)
            {
                var name = $"test-{i}";
                var problem = GetRandomString(10);

                data.Add(Metadata.Terms.DetailedResult.With(new { name, problem }));
            }

            return new ReportResults()
            {
                Type = ReportResultsType.StringList,
                Status = ReportResultsStatus.Information,
                Summary = Metadata.Terms.InformationSummary.With(new { issueCount }),
                Data = data
            };
        }

        private string GetRandomString(int size)
        {
            var builder = new StringBuilder();

            char ch;

            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }

            return builder.ToString();
        }
    }
}