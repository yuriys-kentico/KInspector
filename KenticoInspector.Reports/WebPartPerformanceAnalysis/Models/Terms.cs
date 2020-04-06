using KenticoInspector.Core.Models;

namespace KenticoInspector.Reports.WebPartPerformanceAnalysis.Models
{
    public class Terms
    {
        public Term GoodSummary { get; set; } = null!;

        public TableNamesTerms TableNames { get; set; } = null!;

        public Term WarningSummary { get; set; } = null!;
    }

    public class TableNamesTerms
    {
        public Term TemplatesWithIssues { get; set; } = null!;

        public Term TreeNodesWithIssues { get; set; } = null!;

        public Term WebPartsWithIssues { get; set; } = null!;
    }
}