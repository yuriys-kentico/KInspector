using KenticoInspector.Core.Models;

namespace KenticoInspector.Reports.ClassTableValidation.Models
{
    public class Terms
    {
        public Term CountIssueFound { get; set; }

        public TableTitlesTerms TableTitles { get; set; }

        public Term GoodSummary { get; set; }
    }

    public class TableTitlesTerms
    {
        public Term DatabaseTablesWithMissingKenticoClasses { get; set; }

        public Term KenticoClassesWithMissingDatabaseTables { get; set; }
    }
}