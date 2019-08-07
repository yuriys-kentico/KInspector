using KenticoInspector.Core.Models;

namespace KenticoInspector.Reports.PageTypeAssignmentAnalysis.Models
{
    public class Terms
    {
        public Term GoodSummary { get; set; }

        public Term WarningSummary { get; set; }

        public TableNamesTerms TableNames { get; set; }
    }

    public class TableNamesTerms
    {
        public Term UnassignedPageTypes { get; set; }
    }
}