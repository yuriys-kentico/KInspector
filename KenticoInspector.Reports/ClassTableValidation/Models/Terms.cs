using KenticoInspector.Core.Models;

namespace KenticoInspector.Reports.ClassTableValidation.Models
{
    public class Terms
    {
        public Term ErrorSummary { get; set; }

        public TableNamesTerms TableNames { get; set; }

        public Term GoodSummary { get; set; }
    }

    public class TableNamesTerms
    {
        public Term DatabaseTablesWithMissingKenticoClasses { get; set; }

        public Term KenticoClassesWithMissingDatabaseTables { get; set; }
    }
}