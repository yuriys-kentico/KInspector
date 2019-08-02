using System;
using System.Collections.Generic;
using System.Linq;

using KenticoInspector.Core;
using KenticoInspector.Core.Constants;
using KenticoInspector.Core.Helpers;
using KenticoInspector.Core.Models;
using KenticoInspector.Core.Services.Interfaces;
using KenticoInspector.Reports.ClassTableValidation.Models;
using KenticoInspector.Reports.ClassTableValidation.Models.Data;

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
            ReportTags.Health
        };

        public override ReportResults GetResults()
        {
            var instance = instanceService.CurrentInstance;

            var instanceDetails = instanceService.GetInstanceDetails(instance);

            var tablesWithMissingClass = databaseService.ExecuteSqlFromFile<InformationSchemaTable>(Scripts.GetInformationSchemaTablesWithMissingClass);

            var tableWhitelist = GetTableWhitelist(instanceDetails.DatabaseVersion);

            var tablesWithMissingClassNotInWhitelist = GetTablesNotInWhitelist(tablesWithMissingClass, tableWhitelist);

            var classesWithMissingTable = databaseService.ExecuteSqlFromFile<CmsClass>(Scripts.GetCmsClassesWithMissingTable);

            return CompileResults(tablesWithMissingClassNotInWhitelist, classesWithMissingTable);
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
                Name = Metadata.Terms.TableNames.DatabaseTablesWithMissingKenticoClasses,
                Rows = tablesWithMissingClass
            };

            var classErrors = classesWithMissingTable.Count();

            var classResults = new TableResult<CmsClass>
            {
                Name = Metadata.Terms.TableNames.KenticoClassesWithMissingDatabaseTables,
                Rows = classesWithMissingTable
            };

            var totalErrors = tableErrors + classErrors;

            return new ReportResults
            {
                Type = ReportResultsType.TableList,
                Status = ReportResultsStatus.Error,
                Summary = Metadata.Terms.ErrorSummary.With(new { totalErrors }),
                Data = new
                {
                    tableResults,
                    classResults
                }
            };
        }

        private IEnumerable<InformationSchemaTable> GetTablesNotInWhitelist(IEnumerable<InformationSchemaTable> tablesWithMissingClass, IEnumerable<string> tableWhitelist)
        {
            if (tableWhitelist.Any())
            {
                return tablesWithMissingClass
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