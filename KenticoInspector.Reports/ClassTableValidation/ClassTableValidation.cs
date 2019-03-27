﻿using KenticoInspector.Core;
using KenticoInspector.Core.Models;
using KenticoInspector.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace KenticoInspector.Reports.ClassTableValidation
{
    class ClassTableValidation : IReport
    {
        readonly IDatabaseService _databaseService;
        readonly IInstanceService _instanceService;

        public ClassTableValidation(IDatabaseService databaseService, IInstanceService instanceService) {
            _databaseService = databaseService;
            _instanceService = instanceService;
        }

        public string Codename => "class-table-validation";

        public IList<Version> CompatibleVersions => new List<Version> {
            new Version("10.0"),
            new Version("11.0")
        };

        public IList<Version> IncompatibleVersions => new List<Version>();

        public string LongDescription => "Compares Kentico Classes against tables in database, and displays non-matching entries. Lists tables without Class, Classes without specified table, and missing Class tables. Excludes those classes, which are not meant to have a table.";

        public string Name => "Class/Table Validation";

        public string ShortDescription => "Validates that Kentico classes and Database tables are properly connected.";

        public IList<string> Tags => new List<string> {
            "Database",
            "Health",
        };

        public ReportResults GetResults(Guid InstanceGuid)
        {
            var instance = _instanceService.GetInstance(InstanceGuid);
            var instanceDetails = _instanceService.GetInstanceDetails(instance);
            _databaseService.ConfigureForInstance(instance);

            var tablesWithMissingClass = GetResultsForTables(instanceDetails);
            var classesWithMissingTable = GetResultsForClasses();

            return CompileResults(tablesWithMissingClass, classesWithMissingTable);
        }

        private static ReportResults CompileResults(IEnumerable<dynamic>tablesWithMissingClass, IEnumerable<dynamic> classesWithMissingTable)
        {
            var tableErrors = tablesWithMissingClass.Count();
            var tableResults = new TableResult<dynamic>()
            {
                Name = "Database tables with missing Kentico classes",
                Rows = tablesWithMissingClass
            };
            
            var classErrors = classesWithMissingTable.Count();
            var classResults = new TableResult<dynamic>()
            {
                Name = "Kentico classes with missing database tables",
                Rows = classesWithMissingTable
            };

            var totalErrors = tableErrors + classErrors;
            
            var results = new ReportResults
            {
                Type = ReportResultsType.TableList.ToString(),
                Data = new { tableResults, classResults }
            };

            switch (totalErrors) {
                case 0:
                    results.Status = ReportResultsStatus.Good.ToString();
                    results.Summary = "No issues found.";
                    break;
                case 1:
                    results.Status = ReportResultsStatus.Error.ToString();
                    results.Summary = "1 issue found.";
                    break;
                default:
                    results.Status = ReportResultsStatus.Error.ToString();
                    results.Summary = $"{totalErrors} issues found.";
                    break;
            }

            return results;
        }

        private IEnumerable<dynamic> GetResultsForClasses()
        {
            var classesWithMissingTable = _databaseService.ExecuteSqlFromFile<dynamic>("ClassTableValidation\\GetClassesWithMissingTable.sql");
            //classesWithMissingTable.TableName = "Kentico Classes with missing database table";
            return classesWithMissingTable;
        }

        private IEnumerable<dynamic> GetResultsForTables(InstanceDetails instanceDetails)
        {
            var tablesWithMissingClass = _databaseService.ExecuteSqlFromFile<dynamic>("ClassTableValidation/GetTablesWithMissingClass.sql");
            //tablesWithMissingClass.TableName = "Database tables with missing Kentico class";

            var tableWhitelist = GetTableWhitelist(instanceDetails.DatabaseVersion);
            if (tableWhitelist.Count > 0)
            {
                tablesWithMissingClass = tablesWithMissingClass.Where(t => !tableWhitelist.Contains(t.TABLE_NAME)).ToList();
            }

            return tablesWithMissingClass;
        }

        private List<string> GetTableWhitelist(Version version)
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