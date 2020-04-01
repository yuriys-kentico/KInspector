using System;
using System.Collections.Generic;
using System.Linq;

using KenticoInspector.Core;
using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Helpers;
using KenticoInspector.Core.Models.Results;
using KenticoInspector.Core.Services.Interfaces;
using KenticoInspector.Reports.PageTypeAssignmentAnalysis.Models;
using KenticoInspector.Reports.PageTypeAssignmentAnalysis.Models.Data;

namespace KenticoInspector.Reports.PageTypeAssignmentAnalysis
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
            ReportTags.ContentModeling,
            ReportTags.Health
        };

        public override ReportResults GetResults()
        {
            var pageTypesNotAssignedToSite = databaseService.ExecuteSqlFromFile<CmsPageType>(Scripts.GetPageTypesNotAssignedToSite);

            return CompileResults(pageTypesNotAssignedToSite);
        }

        private ReportResults CompileResults(IEnumerable<CmsPageType> pageTypesNotAssignedToSite)
        {
            if (!pageTypesNotAssignedToSite.Any())
            {
                return new ReportResults(ReportResultsStatus.Good)
                {
                    Summary = Metadata.Terms.GoodSummary
                };
            }

            var unassignedPageTypeCount = pageTypesNotAssignedToSite.Count();

            return new ReportResults(ReportResultsStatus.Warning)
            {
                Summary = Metadata.Terms.WarningSummary.With(new { unassignedPageTypeCount }),
                Data = pageTypesNotAssignedToSite.AsResult().WithLabel(Metadata.Terms.TableNames.UnassignedPageTypes)
            };
        }
    }
}