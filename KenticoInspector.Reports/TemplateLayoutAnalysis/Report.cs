using System;
using System.Collections.Generic;
using System.Linq;

using KenticoInspector.Core;
using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Helpers;
using KenticoInspector.Core.Models;
using KenticoInspector.Core.Services.Interfaces;
using KenticoInspector.Reports.TemplateLayoutAnalysis.Models;
using KenticoInspector.Reports.TemplateLayoutAnalysis.Models.Data;
using KenticoInspector.Reports.TemplateLayoutAnalysis.Models.Results;

namespace KenticoInspector.Reports.TemplateLayoutAnalysis
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
            ReportTags.Information,
            ReportTags.PortalEngine
        };

        public override ReportResults GetResults()
        {
            var pageTemplates = databaseService.ExecuteSqlFromFile<CmsPageTemplate>(Scripts.GetCmsPageTemplates);

            var identicalPageTemplateResults = GetIdenticalPageTemplateResults(pageTemplates);

            return CompileResults(identicalPageTemplateResults);
        }

        private IEnumerable<IdenticalPageTemplateResult> GetIdenticalPageTemplateResults(IEnumerable<CmsPageTemplate> pageTemplates)
        {
            return pageTemplates
                .GroupBy(
                    pageTemplate => pageTemplate.PageTemplateLayout,
                    pageTemplate => $"{pageTemplate.PageTemplateCodeName} ({pageTemplate.PageTemplateID})"
                )
                .Where(pageTemplate => pageTemplate.Count() > 1)
                .Select(identicalPageTemplates => new IdenticalPageTemplateResult(identicalPageTemplates.Key, identicalPageTemplates.ToList()))
                .ToList();
        }

        private ReportResults CompileResults(IEnumerable<IdenticalPageTemplateResult> identicalPageTemplateResults)
        {
            if (!identicalPageTemplateResults.Any())
            {
                return new ReportResults
                {
                    Status = ReportResultsStatus.Good,
                    Summary = Metadata.Terms.GoodSummary
                };
            }

            var count = identicalPageTemplateResults.Count();

            var data = new TableResult<IdenticalPageTemplateResult>()
            {
                Name = Metadata.Terms.TableNames.IdenticalPageLayouts,
                Rows = identicalPageTemplateResults
            };

            return new ReportResults
            {
                Type = ReportResultsType.Table,
                Status = ReportResultsStatus.Information,
                Summary = Metadata.Terms.InformationSummary.With(new { count }),
                Data = data
            };
        }
    }
}