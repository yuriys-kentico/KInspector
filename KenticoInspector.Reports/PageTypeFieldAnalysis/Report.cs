﻿using System.Collections.Generic;
using System.Linq;

using KenticoInspector.Core.Instances.Services;
using KenticoInspector.Core.Modules;
using KenticoInspector.Core.Modules.Models.Results;
using KenticoInspector.Core.Modules.Models.Results.Data;
using KenticoInspector.Modules;
using KenticoInspector.Reports.PageTypeFieldAnalysis.Models;
using KenticoInspector.Reports.PageTypeFieldAnalysis.Models.Data;

using static KenticoInspector.Core.Modules.Models.Tags;

namespace KenticoInspector.Reports.PageTypeFieldAnalysis
{
    public class Report : AbstractReport<Terms>
    {
        private readonly IDatabaseService databaseService;

        public Report(IDatabaseService databaseService)

        {
            this.databaseService = databaseService;
        }

        [Tags(
            ContentModeling,
            Health
            )]
        [SupportsVersions("10 - 12.0")]
        public override ReportResults GetResults()
        {
            var pagetypeFields = databaseService
                .ExecuteSqlFromFile<CmsPageTypeField>(Scripts.GetCmsPageTypeFields);

            var fieldsWithMismatchedTypes = CheckForMismatchedTypes(pagetypeFields);

            return CompileResults(fieldsWithMismatchedTypes);
        }

        private ReportResults CompileResults(IEnumerable<CmsPageTypeField> fieldsWithMismatchedTypes)
        {
            if (!fieldsWithMismatchedTypes.Any())
                return new ReportResults(ResultsStatus.Good)
                {
                    Summary = Metadata.Terms.Summaries.Good
                };

            var fieldResultCount = fieldsWithMismatchedTypes.Count();

            var results = new ReportResults
            {
                Summary = Metadata.Terms.Summaries.Information.With(
                    new
                    {
                        fieldResultCount
                    }
                    ),
                Data = fieldsWithMismatchedTypes.AsResult()
                    .WithLabel(Metadata.Terms.TableTitles.MatchingPageTypeFieldsWithDifferentDataTypes)
            };

            return results;
        }

        private IEnumerable<CmsPageTypeField> CheckForMismatchedTypes(IEnumerable<CmsPageTypeField> pagetypeFields)
        {
            var fieldsWithMismatchedTypes = pagetypeFields
                .Distinct()
                .GroupBy(x => x.FieldName)
                .Where(g => g.Count() > 1)
                .SelectMany(g => g)
                .OrderBy(i => i.FieldName);

            return fieldsWithMismatchedTypes;
        }
    }
}