using System;
using System.Collections.Generic;
using System.Linq;

using KenticoInspector.Core;
using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Helpers;
using KenticoInspector.Core.Models.Results;
using KenticoInspector.Core.Services.Interfaces;
using KenticoInspector.Reports.UnusedPageTypeSummary.Models;
using KenticoInspector.Reports.UnusedPageTypeSummary.Models.Data;

namespace KenticoInspector.Reports.UnusedPageTypeSummary
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
            ReportTags.ContentModeling
        };

        public override ReportResults GetResults()
        {
            var classesNotInViewCmsTreeJoined = databaseService.ExecuteSqlFromFile<CmsClass>(Scripts.GetCmsClassNotInViewCmsTreeJoined);

            return CompileResults(classesNotInViewCmsTreeJoined);
        }

        private ReportResults CompileResults(IEnumerable<CmsClass> classesNotInViewCmsTreeJoined)
        {
            if (!classesNotInViewCmsTreeJoined.Any())
            {
                return new ReportResults(ReportResultsStatus.Good)
                {
                    Summary = Metadata.Terms.GoodSummary
                };
            }

            var count = classesNotInViewCmsTreeJoined.Count();

            return new ReportResults(ReportResultsStatus.Information)
            {
                Summary = Metadata.Terms.InformationSummary.With(new { count }),
                Data = classesNotInViewCmsTreeJoined.AsResult().WithLabel(Metadata.Terms.TableNames.UnusedPageTypes)
            };
        }
    }
}