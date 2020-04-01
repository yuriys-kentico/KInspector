using System;
using System.Collections.Generic;
using System.Linq;

using KenticoInspector.Core;
using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Helpers;
using KenticoInspector.Core.Models.Results;
using KenticoInspector.Core.Services.Interfaces;
using KenticoInspector.Reports.ClassTableValidation.Models;
using KenticoInspector.Reports.ClassTableValidation.Models.Data;

namespace KenticoInspector.Reports.ClassTableValidation
{
    public class Report : AbstractReport<Terms>
    {
        private readonly IDatabaseService databaseService;
        private readonly IInstanceService instanceService;

        public override IList<Version> CompatibleVersions => VersionHelper.GetVersionList("10", "11");

        public override IList<string> Tags => new List<string>
        {
            ReportTags.Health
        };

        public Report(
            IDatabaseService databaseService,
            IInstanceService instanceService,
            IReportMetadataService reportMetadataService
            ) : base(reportMetadataService)
        {
            this.databaseService = databaseService;
            this.instanceService = instanceService;
        }

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
                return new ReportResults(ReportResultsStatus.Good)
                {
                    Summary = Metadata.Terms.Summaries.Good
                };
            }

            var tablesWithMissingClassCount = tablesWithMissingClass.Count();
            var cmsClassesWithMissingTableCount = cmsClassesWithMissingTable.Count();

            return new ReportResults(ReportResultsStatus.Error)
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