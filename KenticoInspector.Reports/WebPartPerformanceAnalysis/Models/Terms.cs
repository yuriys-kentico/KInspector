using KenticoInspector.Core.Models;

namespace KenticoInspector.Reports.WebPartPerformanceAnalysis.Models
{
    public class Terms
    {
        public Term GoodSummary { get; set; }

        public TableNamesTerms TableNames { get; set; }

        public Term WarningSummary { get; set; }
    }

    public class TableNamesTerms
    {
        public Term TemplatesWithIssues { get; set; }

        public Term TreeNodesWithIssues { get; set; }

        public Term WebPartsWithIssues { get; set; }
    }
}