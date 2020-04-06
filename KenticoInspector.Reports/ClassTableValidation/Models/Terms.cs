using KenticoInspector.Core.Models;

namespace KenticoInspector.Reports.ClassTableValidation.Models
{
    public class Terms
    {
        public Summaries Summaries { get; set; } = null!;

        public TableTitles TableTitles { get; set; } = null!;
    }

    public class Summaries
    {
        public Term Error { get; set; } = null!;

        public Term Good { get; set; } = null!;
    }

    public class TableTitles
    {
        public Term DatabaseTablesWithMissingKenticoClasses { get; set; } = null!;

        public Term KenticoClassesWithMissingDatabaseTables { get; set; } = null!;
    }
}