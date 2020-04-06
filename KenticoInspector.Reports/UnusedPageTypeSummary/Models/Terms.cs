using KenticoInspector.Core.Models;

namespace KenticoInspector.Reports.UnusedPageTypeSummary.Models
{
    public class Terms
    {
        public Term GoodSummary { get; set; } = null!;

        public Term InformationSummary { get; set; } = null!;

        public TableNamesTerms TableNames { get; set; } = null!;
    }

    public class TableNamesTerms
    {
        public Term UnusedPageTypes { get; set; } = null!;
    }
}