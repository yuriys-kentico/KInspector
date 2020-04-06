using KenticoInspector.Core.Models;

namespace KenticoInspector.Reports.ColumnFieldValidation.Models
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
        public Term ClassesWithAddedFields { get; set; } = null!;

        public Term TablesWithAddedColumns { get; set; } = null!;
    }
}