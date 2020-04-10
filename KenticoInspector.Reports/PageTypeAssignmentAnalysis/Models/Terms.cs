using KenticoInspector.Core.TokenExpressions.Models;

namespace KenticoInspector.Reports.PageTypeAssignmentAnalysis.Models
{
    public class Terms
    {
        public Term GoodSummary { get; set; } = null!;

        public Term WarningSummary { get; set; } = null!;

        public TableNamesTerms TableNames { get; set; } = null!;
    }

    public class TableNamesTerms
    {
        public Term UnassignedPageTypes { get; set; } = null!;
    }
}