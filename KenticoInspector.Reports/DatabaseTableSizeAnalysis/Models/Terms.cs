using KenticoInspector.Core.TokenExpressions.Models;

namespace KenticoInspector.Reports.DatabaseTableSizeAnalysis.Models
{
    public class Terms
    {
        public Term InformationSummary { get; set; } = null!;

        public TableNamesTerms TableNames { get; set; } = null!;
    }

    public class TableNamesTerms
    {
        public Term Top25Results { get; set; } = null!;
    }
}