using KenticoInspector.Core;
using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Helpers;
using KenticoInspector.Core.Models;
using KenticoInspector.Core.Services.Interfaces;
using KenticoInspector.Reports.ClassTableValidation.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KenticoInspector.Reports.ClassTableValidation
{
    public class Report : AbstractReport<Terms>
    {
        private readonly IDatabaseService databaseService;
        private readonly IInstanceService instanceService;

        public Report(IDatabaseService databaseService, IInstanceService instanceService, IReportMetadataService reportMetadataService) : base(reportMetadataService)
        {
            this.databaseService = databaseService;
            this.instanceService = instanceService;
        }

        public override IList<Version> CompatibleVersions => VersionHelper.GetVersionList("10", "11");

        public override IList<string> Tags => new List<string>
        {
            ReportTags.Health,
        };

        public override ReportResults GetResults()
        {
            var instance = instanceService.CurrentInstance;

            var instanceDetails = instanceService.GetInstanceDetails(instance);

            var tablesWithMissingClass = GetResultsForTables(instanceDetails);
            var classesWithMissingTable = GetResultsForClasses();

            return CompileResults(tablesWithMissingClass, classesWithMissingTable);
        }

        private ReportResults CompileResults(IEnumerable<InformationSchemaTable> tablesWithMissingClass, IEnumerable<CmsClass> classesWithMissingTable)
        {
            if (!tablesWithMissingClass.Any() && !classesWithMissingTable.Any())
            {
                return new ReportResults()
                {
                    Type = ReportResultsType.String,
                    Status = ReportResultsStatus.Good,
                    Summary = Metadata.Terms.GoodSummary
                };
            }

            var tableErrors = tablesWithMissingClass.Count();

            var tableResults = new TableResult<InformationSchemaTable>
            {
                Name = Metadata.Terms.TableTitles.DatabaseTablesWithMissingKenticoClasses,
                Rows = tablesWithMissingClass
            };

            var classErrors = classesWithMissingTable.Count();

            var classResults = new TableResult<CmsClass>
            {
                Name = Metadata.Terms.TableTitles.KenticoClassesWithMissingDatabaseTables,
                Rows = classesWithMissingTable
            };

            var totalErrors = tableErrors + classErrors;

            return new ReportResults
            {
                Type = ReportResultsType.TableList,
                Status = ReportResultsStatus.Error,
                Summary = Metadata.Terms.CountIssueFound.With(new { totalErrors }),
                Data = new
                {
                    tableResults,
                    classResults
                }
            };
        }

        private IEnumerable<CmsClass> GetResultsForClasses()
        {
            var classesWithMissingTable = databaseService.ExecuteSqlFromFile<CmsClass>(Scripts.GetCmsClass);

            return classesWithMissingTable;
        }

        private IEnumerable<InformationSchemaTable> GetResultsForTables(InstanceDetails instanceDetails)
        {
            var tablesWithMissingClass = databaseService.ExecuteSqlFromFile<InformationSchemaTable>(Scripts.GetInformationSchemaTables);

            var tableWhitelist = GetTableWhitelist(instanceDetails.DatabaseVersion);

            if (tableWhitelist.Count() > 0)
            {
                tablesWithMissingClass = tablesWithMissingClass
                    .Where(t => !tableWhitelist
                        .Contains(t.TableName)
                    )
                    .ToList();
            }

            return tablesWithMissingClass;
        }

        private IEnumerable<string> GetTableWhitelist(Version version)
        {
            var whitelist = new List<string>();

            if (version.Major >= 10)
            {
                whitelist.Add("CI_Migration");
            }

            return whitelist;
        }
    }
}