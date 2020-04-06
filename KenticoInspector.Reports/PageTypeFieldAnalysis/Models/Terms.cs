using KenticoInspector.Core.Models;

namespace KenticoInspector.Reports.PageTypeFieldAnalysis.Models
{
    public class Terms
    {
        public Summaries Summaries { get; set; } = null!;

        public TableTitles TableTitles { get; set; } = null!;
    }

    public class Summaries
    {
        public Term Information { get; set; } = null!;

        public Term Good { get; set; } = null!;
    }

    public class TableTitles
    {
        public Term MatchingPageTypeFieldsWithDifferentDataTypes { get; set; } = null!;
    }
}