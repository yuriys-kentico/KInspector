using KenticoInspector.Core.Models;

namespace KenticoInspector.Reports.DatabaseTableSizeAnalysis.Models
{
    public class Terms
    {
        public Term InformationSummary { get; set; }

        public TableNamesTerms TableNames { get; set; }
    }

    public class TableNamesTerms
    {
        public Term Top25Results { get; set; }
    }
}