using System.Collections.Generic;
using System.Linq;

using KenticoInspector.Core.Instances.Services;
using KenticoInspector.Core.Modules;
using KenticoInspector.Core.Modules.Models.Results;
using KenticoInspector.Core.Modules.Models.Results.Data;
using KenticoInspector.Modules;
using KenticoInspector.Reports.TemplateLayoutAnalysis.Models;
using KenticoInspector.Reports.TemplateLayoutAnalysis.Models.Data;
using KenticoInspector.Reports.TemplateLayoutAnalysis.Models.Results;

using static KenticoInspector.Core.Modules.Models.Tags;

namespace KenticoInspector.Reports.TemplateLayoutAnalysis
{
    public class Report : AbstractReport<Terms>
    {
        private readonly IDatabaseService databaseService;

        public Report(IDatabaseService databaseService)
        {
            this.databaseService = databaseService;
        }

        [Tags(ContentModeling, PortalEngine)]
        [SupportsVersions("10 - 12.0")]
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
                .Select(identicalPageTemplates => new IdenticalPageTemplateResult
                {
                    PageTemplateLayout = identicalPageTemplates.Key,
                    PageTemplateCodenamesAndIds = string.Join(", ", identicalPageTemplates.ToList())
                })
                .ToList();
        }

        private ReportResults CompileResults(IEnumerable<IdenticalPageTemplateResult> identicalPageTemplateResults)
        {
            if (!identicalPageTemplateResults.Any())
            {
                return new ReportResults(ResultsStatus.Good)
                {
                    Summary = Metadata.Terms.GoodSummary
                };
            }
            var count = identicalPageTemplateResults.Count();

            return new ReportResults(ResultsStatus.Information)
            {
                Summary = Metadata.Terms.InformationSummary.With(new { count }),
                Data = identicalPageTemplateResults.AsResult().WithLabel(Metadata.Terms.TableNames.IdenticalPageLayouts)
            };
        }
    }
}