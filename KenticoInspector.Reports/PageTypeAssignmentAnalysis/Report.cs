using System;
using System.Collections.Generic;
using System.Linq;

using KenticoInspector.Core;
using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Helpers;
using KenticoInspector.Core.Models;
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
            ReportTags.Health,
            ReportTags.Consistency
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
                return new ReportResults
                {
                    Status = ReportResultsStatus.Good,
                    Summary = Metadata.Terms.GoodSummary
                };
            }

            var unassignedPageTypeCount = pageTypesNotAssignedToSite.Count();

            var data = new TableResult<CmsPageType>()
            {
                Name = Metadata.Terms.TableNames.UnassignedPageTypes,
                Rows = pageTypesNotAssignedToSite
            };

            return new ReportResults
            {
                Status = ReportResultsStatus.Warning,
                Summary = Metadata.Terms.WarningSummary.With(new { unassignedPageTypeCount }),
                Type = ReportResultsType.Table,
                Data = data
            };
        }
    }
}