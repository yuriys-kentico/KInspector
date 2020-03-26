using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using KenticoInspector.Core;
using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Helpers;
using KenticoInspector.Core.Models;
using KenticoInspector.Core.Models.Results;
using KenticoInspector.Core.Services.Interfaces;
using KenticoInspector.Reports.RobotsTxtConfigurationSummary.Models;

namespace KenticoInspector.Reports.RobotsTxtConfigurationSummary
{
    public class Report : AbstractReport<Terms>
    {
        private readonly IInstanceService instanceService;
        private readonly HttpClient httpClient;

        public Report(IInstanceService instanceService, IReportMetadataService reportMetadataService, HttpClient httpClient = null) : base(reportMetadataService)
        {
            this.instanceService = instanceService;
            this.httpClient = httpClient ?? new HttpClient();
        }

        public override IList<Version> CompatibleVersions => VersionHelper.GetVersionList("10", "11", "12");

        public override IList<string> Tags => new List<string>
        {
            ReportTags.Information,
            ReportTags.SEO,
        };

        public override ReportResults GetResults()
        {
            var instance = instanceService.CurrentInstance;

            var instanceUri = new Uri(instance.Url);

            var testUri = new Uri(instanceUri, DefaultKenticoPaths.RobotsTxtRelative);

            var uriStatusCode = GetUriStatusCode(testUri).Result;

            return CompileResults(testUri, uriStatusCode, HttpStatusCode.OK);
        }

        private async Task<HttpStatusCode> GetUriStatusCode(Uri testUri)
        {
            HttpResponseMessage response = await httpClient.GetAsync(testUri);

            return response.StatusCode;
        }

        private ReportResults CompileResults(Uri testUri, HttpStatusCode uriStatusCode, HttpStatusCode statusCodeWithoutIssues)
        {
            if (uriStatusCode == statusCodeWithoutIssues)
            {
                return new ReportResults(ReportResultsStatus.Good)
                {
                    Summary = Metadata.Terms.GoodSummary.With(new { testUri })
                };
            }

            int uriStatusInteger = (int)uriStatusCode;

            return new ReportResults(ReportResultsStatus.Warning)
            {
                Summary = Metadata.Terms.WarningSummary.With(new { testUri, uriStatusInteger })
            };
        }
    }
}