using System.Collections.Generic;
using System.Linq;

using KenticoInspector.Core;
using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Models.Results;
using KenticoInspector.Core.Services;
using KenticoInspector.Reports.UnusedPageTypeSummary.Models;
using KenticoInspector.Reports.UnusedPageTypeSummary.Models.Data;

namespace KenticoInspector.Reports.UnusedPageTypeSummary
{
    public class Report : AbstractReport<Terms>
    {
        private readonly IDatabaseService databaseService;

        public Report(IDatabaseService databaseService)
        {
            this.databaseService = databaseService;
        }

        public override IList<string> Tags => new List<string>
        {
            ReportTags.ContentModeling
        };

        [SupportsVersions("10 - 12.0")]
        public override ReportResults GetResults()
        {
            var classesNotInViewCmsTreeJoined = databaseService.ExecuteSqlFromFile<CmsClass>(Scripts.GetCmsClassNotInViewCmsTreeJoined);

            return CompileResults(classesNotInViewCmsTreeJoined);
        }

        private ReportResults CompileResults(IEnumerable<CmsClass> classesNotInViewCmsTreeJoined)
        {
            if (!classesNotInViewCmsTreeJoined.Any())
            {
                return new ReportResults(ResultsStatus.Good)
                {
                    Summary = Metadata.Terms.GoodSummary
                };
            }

            var count = classesNotInViewCmsTreeJoined.Count();

            return new ReportResults(ResultsStatus.Information)
            {
                Summary = Metadata.Terms.InformationSummary.With(new { count }),
                Data = classesNotInViewCmsTreeJoined.AsResult().WithLabel(Metadata.Terms.TableNames.UnusedPageTypes)
            };
        }
    }
}