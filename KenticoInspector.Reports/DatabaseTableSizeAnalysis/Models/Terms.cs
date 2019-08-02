using KenticoInspector.Core.Models;

namespace KenticoInspector.Reports.DatabaseTableSizeAnalysis.Models
{
    public class Terms
    {
        public Term InformationSummary { get; set; }

        public TableNames TableNames { get; set; }
    }

    public class TableNames
    {
        public Term Top25Results { get; set; }
    }
}