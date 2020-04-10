using System;
using System.Collections.Generic;
using System.Linq;

using KenticoInspector.Core.Instances.Services;
using KenticoInspector.Core.Modules;
using KenticoInspector.Core.Modules.Models.Results;
using KenticoInspector.Core.Modules.Models.Results.Data;
using KenticoInspector.Modules;
using KenticoInspector.Reports.ClassTableValidation.Models;
using KenticoInspector.Reports.ClassTableValidation.Models.Data;

using static KenticoInspector.Core.Modules.Models.Tags;

namespace KenticoInspector.Reports.ClassTableValidation
{
    public class Report : AbstractReport<Terms>
    {
        private readonly IDatabaseService databaseService;
        private readonly IInstanceService instanceService;

        public Report(
            IDatabaseService databaseService,
            IInstanceService instanceService
            )
        {
            this.databaseService = databaseService;
            this.instanceService = instanceService;
        }

        [Tags(Health)]
        [SupportsVersions("10 - 12.0")]
        public override ReportResults GetResults()
        {
            var instance = instanceService.CurrentInstance;

            var instanceDetails = instanceService.GetInstanceDetails(instance);

            var tableWhitelist = GetTableWhitelist(instanceDetails.DatabaseVersion);

            var tablesWithMissingClass = databaseService.ExecuteSqlFromFile<DatabaseTable>(
                Scripts.GetTablesWithMissingClass
            );

            var tablesWithMissingClassNotInWhitelist = GetTablesNotInWhitelist(tablesWithMissingClass, tableWhitelist);

            var cmsClassesWithMissingTable = databaseService.ExecuteSqlFromFile<CmsClass>(
                Scripts.GetCmsClassesWithMissingTable
            );

            return CompileResults(tablesWithMissingClassNotInWhitelist, cmsClassesWithMissingTable);
        }

        private static IEnumerable<string> GetTableWhitelist(Version version)
        {
            if (version.Major >= 10)
            {
                yield return "CI_Migration";
            }
        }

        private static IEnumerable<DatabaseTable> GetTablesNotInWhitelist(
            IEnumerable<DatabaseTable> tablesWithMissingClass,
            IEnumerable<string> tableWhitelist
            )
        {
            if (tableWhitelist.Any())
            {
                return tablesWithMissingClass
                    .Where(table => !tableWhitelist.Contains(table.TableName));
            }

            return tablesWithMissingClass;
        }

        private ReportResults CompileResults(
            IEnumerable<DatabaseTable> tablesWithMissingClass,
            IEnumerable<CmsClass> cmsClassesWithMissingTable
            )
        {
            if (!tablesWithMissingClass.Any() && !cmsClassesWithMissingTable.Any())
            {
                return new ReportResults(ResultsStatus.Good)
                {
                    Summary = Metadata.Terms.Summaries.Good
                };
            }

            var tablesWithMissingClassCount = tablesWithMissingClass.Count();
            var cmsClassesWithMissingTableCount = cmsClassesWithMissingTable.Count();

            return new ReportResults(ResultsStatus.Error)
            {
                Summary = Metadata.Terms.Summaries.Error.With(new { tablesWithMissingClassCount, cmsClassesWithMissingTableCount }),
                Data =
                {
                    tablesWithMissingClass.AsResult().WithLabel(Metadata.Terms.TableTitles.DatabaseTablesWithMissingKenticoClasses),
                    cmsClassesWithMissingTable.AsResult().WithLabel(Metadata.Terms.TableTitles.KenticoClassesWithMissingDatabaseTables)
                }
            };
        }
    }
}